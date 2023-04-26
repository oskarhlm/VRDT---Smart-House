from pvlib import location
from pvlib import irradiance
import pandas as pd
from matplotlib import pyplot as plt
import datetime
import numpy as np


# https://www.enfsolar.com/pv/panel-datasheet/crystalline/41749
SPR_Max3_400 = {
    "Pmax": 400,
    'efficiency': 0.226,
    # Temperature Coefficient of Pmax, reduction per degree celsius offset from 25 degrees celsius
    'temp_coeff': -0.0029,
    'panel_size': (1.690*1.046)  # Square meters with panel dimensions
}


now = datetime.datetime.now()

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

# Plot GHI vs. POA for winter and summer


def plot_irradiance(comparison, summer_irr, current_irr):
    if comparison:
        fig, (ax1, ax2) = plt.subplots(1, 2, sharey=True)
        summer_irr['GHI'].plot(ax=ax1, label='GHI')
        summer_irr['POA'].plot(ax=ax1, label='POA')
        current_irr['GHI'].plot(ax=ax2, label='GHI')
        current_irr['POA'].plot(ax=ax2, label='POA')
        ax1.set_xlabel('Time of day (Summer)')
        ax2.set_xlabel('Time of day (Current)')
        ax1.set_ylabel('Irradiance ($W/m^2$)')
        ax1.legend()
        ax2.legend()
    else:
        fig, ax1 = plt.subplots(1, 1)
        current_irr['GHI'].plot(ax=ax1, label='GHI')
        current_irr['POA'].plot(ax=ax1, label='POA')
        ax1.set_xlabel('Time of day (Current)')
        ax1.set_ylabel('Irradiance ($W/m^2$)')
        ax1.legend()

    plt.show()


def calculate_irr(panel_tilt=25, panel_azimuth=180, current_time='12:00'):
    # Get irradiance data for summer and winter solstice, assuming 25 degree tilt
    # and a south facing array
    summer_irradiance = get_irradiance(
        site, '06-20-2020', panel_tilt, panel_azimuth)
    current_irradiance = get_irradiance(
        site, str(now.date()), panel_tilt, panel_azimuth)
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


def approximated_outp(model=SPR_Max3_400, temp=20, num_panels=1):
    # calculate_irr takes inn solar panel tilt and azimuth degree as arguments
    summer_irradiance, current_irradiance = calculate_irr()
    POA_irradiance = current_irradiance.at[time_rounded, 'POA']

    print('Current POA irradiance:', POA_irradiance)
    print('Offset from optimal solarpanel tilt (GHI - POA):',
          np.abs(current_irradiance.at[time_rounded, 'GHI'] - POA_irradiance))

    temp_eff = panel_temp_efficiency(model, temp)
    print('Approx efficiency at', temp, ': ', temp_eff)

    power_outp = current_irradiance.at[time_rounded,
                                       'POA'] * temp_eff * (model['panel_size']*num_panels)

    return power_outp, summer_irradiance, current_irradiance


# row = current_irradiance.iloc[10]
# print(row['POA'])
approx_power, summer_irradiance, current_irradiance = approximated_outp()

print('Approximted power output:', approx_power)

plot_irradiance(False, summer_irradiance, current_irradiance)
