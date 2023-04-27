from pvlib import location, irradiance
import pandas as pd
from matplotlib import pyplot as plt
from matplotlib.cm import ScalarMappable
import datetime
import numpy as np
from scipy.integrate import trapz, simps
import requests

# https://www.enfsolar.com/pv/panel-datasheet/crystalline/41749
SPR_Max3_400 = {
    "name": 'SPR_Max3_400',
    "Pmax": 400,
    'efficiency': 0.226,
    # Temperature Coefficient of Pmax, reduction per degree celsius offset from 25 degrees celsius
    'temp_coeff': -0.0029,
    'panel_size': (1.690*1.046)  # Square meters with panel dimensions
}


now = datetime.datetime.now()

# --------------------------------------#
#         Electricity price            #
# --------------------------------------#


def day_month_formatter(num):
    if len(str(num)) == 1:
        return '0'+str(num)
    return str(num)


str_m = day_month_formatter(now.month)
str_d = day_month_formatter(now.day)
date_to_string = str(now.year) + '/' + str_m + '-' + str_d

url = 'https://www.hvakosterstrommen.no/api/v1/prices/' + date_to_string + '_NO3.json'
response = requests.get(url)
json_response = response.json()

nok_per_kWh = [element['NOK_per_kWh'] for element in json_response]

# --------------------------------------#
#     Solar panel output estimation    #
# --------------------------------------#

# Rounds the timefrequency to 10 min intervals
rounded = now - datetime.timedelta(minutes=now.minute % 10,
                                   seconds=now.second,
                                   microseconds=now.microsecond)  # Round to nearest 10 minute interval

time_rounded = rounded.strftime('%H:%M')

# Timezone and coordinates
tz = 'Europe/Oslo'
lat, lon = 10.5030113, 63.4021589

# Create location object to store lat, lon, timezone
site = location.Location(lat, lon, tz=tz)

# Calculate clear-sky GHI and transpose to plane of array
# Define a function so that we can re-use the sequence of operations with different locations


def get_irradiance(site_location, date, tilt, surface_azimuth):
    # Creates one day's worth of 10 min intervals
    times = pd.date_range(date, freq='10min', periods=6*24,
                          tz=site_location.tz)
    # Generate clearsky data using the Ineichen model, which is the default
    # The get_clearsky method returns a dataframe with values for GHI, DNI, and DHI
    clearsky = site_location.get_clearsky(times)
    # Get solar azimuth and zenith to pass to the transposition function
    solar_position = site_location.get_solarposition(times=times)
    # Use the get_total_irradiance function to transpose the GHI to POA
    POA_irradiance = irradiance.get_total_irradiance(
        surface_tilt=tilt,
        surface_azimuth=surface_azimuth,
        dni=clearsky['dni'],
        ghi=clearsky['ghi'],
        dhi=clearsky['dhi'],
        solar_zenith=solar_position['apparent_zenith'],
        solar_azimuth=solar_position['azimuth'])
    # Return DataFrame with only GHI and POA
    return pd.DataFrame({'GHI': clearsky['ghi'],
                         'POA': POA_irradiance['poa_global']})


def calculate_irr(panel_tilt=25, panel_azimuth=180, time=now):
    # Get irradiance data for summer and winter solstice, assuming 25 degree tilt
    # and a south facing array
    summer_irradiance = get_irradiance(
        site, '06-20-2020', panel_tilt, panel_azimuth)
    current_irradiance = get_irradiance(
        site, str(time.date()), panel_tilt, panel_azimuth)
    # Convert Dataframe Indexes to Hour:Minute format to make plotting easier
    summer_irradiance.index = summer_irradiance.index.strftime("%H:%M")
    current_irradiance.index = current_irradiance.index.strftime("%H:%M")

    return summer_irradiance, current_irradiance


def panel_temp_efficiency(model, temp=25):
    if temp >= 25:
        temp_reduction = (temp-25)*model['temp_coeff']
    else:
        temp_reduction = (25-temp)*model['temp_coeff']
    new_eff = model['efficiency'] + temp_reduction
    return new_eff


def approximated_outp(model=SPR_Max3_400, temp=20, panel_tilt=25, panel_azimuth=18, time=now, time_rounded=time_rounded):
    # calculate_irr takes inn solar panel tilt and azimuth degree as arguments
    summer_irradiance, current_irradiance = calculate_irr(
        panel_tilt, panel_azimuth, time)
    temp_eff = panel_temp_efficiency(model, temp)
#     print('Approx efficiency at {t} \N{DEGREE SIGN}C: {e:2f} %'.format(t=temp, e=temp_eff*100))
    current_power_outp = current_irradiance.at[time_rounded,
                                               'POA'] * temp_eff * (model['panel_size'])
    daily_power_outp = current_irradiance['POA'] * \
        temp_eff * (model['panel_size'])
    return current_power_outp, daily_power_outp, summer_irradiance, current_irradiance, temp_eff


# ----------------------------#
# Plot GHI vs. POA for winter and summer
# ----------------------------#
def plot_irradiance(comparison=False, model=SPR_Max3_400, temp=20, num_panels=4, panel_tilt=25, panel_azimuth=180, time=now):
    rounded = time - datetime.timedelta(minutes=time.minute % 10,
                                        seconds=time.second,
                                        microseconds=time.microsecond)  # Round to nearest 10 minute interval
    time_rounded = rounded.strftime('%H:%M')
    _, _, summer_irradiance, current_irradiance, _, _, _, _, _ = setup(
        model, temp, num_panels, panel_tilt, panel_azimuth, time, rounded, time_rounded)
    if comparison:
        fig, (ax1, ax2) = plt.subplots(1, 2, sharey=True)
        summer_irradiance['GHI'].plot(ax=ax1, label='GHI')
        summer_irradiance['POA'].plot(ax=ax1, label='POA')
        summer_irradiance['GHI'].plot(ax=ax2, label='GHI')
        summer_irradiance['POA'].plot(ax=ax2, label='POA')
        ax1.set_xlabel('Time of day (Summer)')
        ax2.set_xlabel('Time of day (Current)')
        ax1.set_ylabel('Irradiance ($W/m^2$)')
        ax1.legend()
        ax2.legend()
        ax1.grid()
        ax2.grid()
    else:
        fig, ax1 = plt.subplots(1, 1)
        current_irradiance['GHI'].plot(ax=ax1, label='GHI')
        current_irradiance['POA'].plot(ax=ax1, label='POA')
        ax1.set_xlabel('Time of day (Current)')
        ax1.set_ylabel('Irradiance ($W/m^2$)')
        ax1.legend()
        ax1.grid()

    plt.show()

# ----------------------------#
# Plot power output vs irradiance
# ----------------------------#


def plot_irr_vs_pow(model=SPR_Max3_400, temp=20, num_panels=4, panel_tilt=25, panel_azimuth=180, time=now):
    rounded = time - datetime.timedelta(minutes=time.minute % 10,
                                        seconds=time.second,
                                        microseconds=time.microsecond)  # Round to nearest 10 minute interval
    time_rounded = rounded.strftime('%H:%M')

    _, daily_approx_power, _, current_irradiance, _, _, _, _, _ = setup(
        model, temp, num_panels, panel_tilt, panel_azimuth, time, rounded, time_rounded)
    x_seq = np.linspace(0, 23, num=len(daily_approx_power))

    fig, axs = plt.subplots(1, 2, sharex=False, sharey=True)
    axs[0].plot(x_seq, current_irradiance['POA'], label='POA', color='black')
    axs[1].plot(x_seq, daily_approx_power, label='Power output {txt} panels'.format(
        txt=num_panels), color='red')
    axs[0].set_xlabel('Time of day')
    axs[1].set_xlabel('Time of day')
    axs[0].set_ylabel('Irradiance ($W/m^2$)')
    axs[1].set_ylabel('Power ($W$)')
    axs[0].legend()
    axs[1].legend()
    axs[0].grid()
    axs[1].grid()
    plt.show()


def plot_current_pow(model=SPR_Max3_400, temp=20, num_panels=4, panel_tilt=25, panel_azimuth=180, time=now):
    rounded = time - datetime.timedelta(minutes=time.minute % 10,
                                        seconds=time.second,
                                        microseconds=time.microsecond)  # Round to nearest 10 minute interval
    time_rounded = rounded.strftime('%H:%M')

    current_approx_power, _, _, _, temp_eff, _, _, _, _ = setup(
        model, temp, num_panels, panel_tilt, panel_azimuth, time, rounded, time_rounded)
    x_labels = ['Power (W)', 'Panel efficiency']
    y_val = [current_approx_power, temp_eff]

    colors = np.random.rand(5)

    fig, (ax1, ax2) = plt.subplots(ncols=2, figsize=(8, 4))

    # Create bar plots with colored bars on each subplot
    ax1.bar(x_labels[0], y_val[0], color='red', alpha=0.2)
    ax2.bar(x_labels[1], y_val[1], color='blue', alpha=0.2)

    ax1.set_ylim([0, 2000])
    ax1.set_ylabel('W')
    ax2.set_ylim([0, 1])
    ax2.set_ylabel('Effeciency')

    # Add a colorbar to each subplot
    sm1 = ScalarMappable(cmap=plt.cm.viridis)
    sm1.set_array(colors)
    plt.colorbar(sm1, ax=ax1)

    sm2 = ScalarMappable(cmap=plt.cm.viridis)
    sm2.set_array(colors)
    plt.colorbar(sm2, ax=ax2)

    plt.show()


def display_statistics(model=SPR_Max3_400, temp=20, num_panels=4, panel_tilt=25, panel_azimuth=180, time=now):
    rounded = time - datetime.timedelta(minutes=time.minute % 10,
                                        seconds=time.second,
                                        microseconds=time.microsecond)  # Round to nearest 10 minute interval
    time_rounded = rounded.strftime('%H:%M')

    current_approx_power, _, _, current_irradiance, _, _, total_power_gen, _, total_money_gen = setup(
        model, temp, num_panels, panel_tilt, panel_azimuth, time, rounded, time_rounded)
    POA_irradiance = current_irradiance.at[time_rounded, 'POA']
    print('#---------#')
    print(' --STATS-- ')
    print('#---------#')
    print('Solar panel model                                           {txt}'.format(
        txt=model['name']))
    print('Number of panels                                            {txt}'.format(
        txt=num_panels))
    print(' ')
    print('Current POA irradiance ({t}):               {txt:.2f} W/m^2'.format(
        t=(rounded), txt=POA_irradiance))
    print('Irradiance offset from optimal solarpanel tilt (GHI - POA): {txt:.2f} W/m^2'.format(
        txt=np.abs(current_irradiance.at[time_rounded, 'GHI'] - POA_irradiance)))
    print(' ')
    print('Current power output   ({t}):               {txt:2f} W'.format(
        t=(rounded), txt=current_approx_power))
    print('Total power generated          {date}:               {power:2f} kWh'.format(
        date=(time.year, time.month, time.day), power=total_power_gen/1000))
    print('Total money generated          {date}:               {money:2f} Kr'.format(
        date=(time.year, time.month, time.day), money=total_money_gen))
    print('#---------#')


def display_current_power(model=SPR_Max3_400, temp=20, num_panels=4, panel_tilt=25, panel_azimuth=180, time=now):
    current_approx_power, _, _, _, _, _, _, _, _ = setup(
        model, temp, num_panels, panel_tilt, panel_azimuth, time)
    print('Current power output {txt:2f} W:'.format(txt=current_approx_power))


def display_power_total(model=SPR_Max3_400, temp=20, num_panels=4, panel_tilt=25, panel_azimuth=180, time=now):
    rounded = time - datetime.timedelta(minutes=time.minute % 10,
                                        seconds=time.second,
                                        microseconds=time.microsecond)  # Round to nearest 10 minute interval
    time_rounded = rounded.strftime('%H:%M')
    _, _, _, _, _, _, total_power_gen, _, _ = setup(
        model, temp, num_panels, panel_tilt, panel_azimuth, time, rounded, time_rounded)
    print('Total power generated {date}: {power:2f} kWh'.format(
        date=(time.year, time.month, time.day), power=total_power_gen/1000))


def display_money_total(model=SPR_Max3_400, temp=20, num_panels=4, panel_tilt=25, panel_azimuth=180, time=now):
    rounded = time - datetime.timedelta(minutes=time.minute % 10,
                                        seconds=time.second,
                                        microseconds=time.microsecond)  # Round to nearest 10 minute interval
    time_rounded = rounded.strftime('%H:%M')

    _, _, _, _, _, _, _, _, total_money_gen = setup(
        model, temp, num_panels, panel_tilt, panel_azimuth, time, rounded, time_rounded)
    print('Total money generated {date}: {money:2f} Kr'.format(
        date=(time.year, time.month, time.day), money=total_money_gen))


def setup(model=SPR_Max3_400, temp=20, num_panels=4, panel_tilt=25, panel_azimuth=180, time=now, rounded=rounded, time_rounded=time_rounded):

    current_approx_power, daily_approx_power, summer_irradiance, current_irradiance, temp_eff = approximated_outp(
        model, temp, num_panels, panel_tilt, panel_azimuth, time, rounded, time_rounded)

    # Find power at every timepoint
    power_val = []
    for i in range(len(daily_approx_power)):
        power_val.append(daily_approx_power[i])

    # Integrate to total power generated
    power_x_seq = list(range(len(power_val)))
    total_power_gen = trapz(power_val, power_x_seq)

    # Find money generated every hour (change modulo if len(nok_per_kWh) != 24, that is, if we get electricity price more than hourly)
    money_gen = []
    j = 0
    for i in range(len(power_x_seq)):
        if i == 0:
            money_gen.append(nok_per_kWh[j]*power_val[i]/1000)
            j += 1
        elif i % 6 == 0:
            money_gen.append(nok_per_kWh[j]*power_val[i]/1000)
            j += 1
    money_gen_x_seq = list(range(len(money_gen)))
    total_money_gen = simps(money_gen, money_gen_x_seq)

    return current_approx_power, daily_approx_power, summer_irradiance, current_irradiance, temp_eff, power_x_seq, total_power_gen, money_gen_x_seq, total_money_gen

# ----------------------------#
# Display statistics / plots
# ----------------------------#

# PARAMETERS:
#
# time - must be on form: time = datetime.datetime.now()
# panel_azimuth and panel_tilt - angles that describe the orientation of the solar panels
# num_panels - number of panels
# temp - temperature (rough correction based on the temperature of the solarpanels)

# display_current_power()
# display_power_total()
# #OBS  Calculation of money does not work...
# display_money_total()


display_statistics()
plot_irradiance(model=SPR_Max3_400, temp=20, num_panels=4,
                panel_tilt=25, panel_azimuth=180, time=now)
plot_irr_vs_pow(model=SPR_Max3_400, temp=20, num_panels=4,
                panel_tilt=25, panel_azimuth=180, time=now)
plot_current_pow(model=SPR_Max3_400, temp=20, num_panels=4,
                 panel_tilt=25, panel_azimuth=180, time=now)
