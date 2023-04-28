from pvlib import location, irradiance
import pandas as pd
from matplotlib import pyplot as plt
from matplotlib.cm import ScalarMappable
import datetime
import numpy as np
import requests
from typing import Tuple, List
from utils import plt_fig_to_pil
from base_pb2 import SolarPanelMessages as T

now = datetime.datetime.now()
tz = 'Europe/Oslo'
lat, lon = 10.5030113, 63.4021589
adil_house_site = location.Location(lat, lon, tz=tz)

# Rounds the timefrequency to 10 min intervals


def rounded_time(time: datetime):
    rounded = now - datetime.timedelta(minutes=now.minute % 10,
                                       seconds=now.second,
                                       microseconds=now.microsecond)
    time_rounded = rounded.strftime('%H:%M')
    return time_rounded


class SolarPanel():

    def __init__(self, panel_dimensions: Tuple[float, float], tilt, azimuth,
                 site: location.Location, temp_coeff, Pmax, effeciency):
        self.panel_dimensions = panel_dimensions
        self.tilt = tilt
        self.azimuth = azimuth
        self.site = site
        self.temp_coeff = temp_coeff
        self.Pmax = Pmax
        self._efficiency = effeciency

    @property
    def panel_size(self):
        return self.panel_dimensions[0] * self.panel_dimensions[1]

    def get_temp_efficiency(self, temp=25):
        if temp >= 25:
            temp_reduction = (temp-25) * self.temp_coeff
        else:
            temp_reduction = (25-temp) * self.temp_coeff
        new_eff = self._efficiency + temp_reduction
        return new_eff

    def get_current_irradiance(self, time=now):
        current_irradiance = get_irradiance(self, time)
        current_irradiance.index = current_irradiance.index.strftime("%H:%M")
        return current_irradiance[rounded_time(time), 'POA']

    def get_current_power_output(self, time=now):
        current_irradiance = self.get_current_irradiance(time)
        return current_irradiance * self.get_temp_efficiency() * self.panel_size


class SPRMax400(SolarPanel):
    """Source: https://www.enfsolar.com/pv/panel-datasheet/crystalline/41749"""

    def __init__(self, tilt, azimuth) -> None:
        super().__init__(panel_dimensions=(1.690, 1.046), tilt=tilt, azimuth=azimuth,
                         site=adil_house_site, temp_coeff=-0.0029, Pmax=400, effeciency=0.226)

    def __str__(self) -> str:
        return 'SPR_Max3_400'


def electricity_prices(time=now):
    def day_month_formatter(num):
        if len(str(num)) == 1:
            return '0'+str(num)
        return str(num)

    str_m = day_month_formatter(time.month)
    str_d = day_month_formatter(time.day)
    date_to_string = str(time.year) + '/' + str_m + '-' + str_d

    url = 'https://www.hvakosterstrommen.no/api/v1/prices/' + date_to_string + '_NO3.json'
    response = requests.get(url)
    json_response = response.json()

    nok_per_kWh = [element['NOK_per_kWh'] for element in json_response]
    return nok_per_kWh


def get_irradiance(panel: SolarPanel, time: datetime):
    """Returns pd.dataframe with GHI and POA values for a given day with 10 min intervals."""

    # Creates one day's worth of 10 min intervals
    times = pd.date_range(str(time.date()), freq='10min', periods=6*24,
                          tz=panel.site.tz)

    # Generate clearsky data using the Ineichen model, which is the default
    # The get_clearsky method returns a dataframe with values for GHI, DNI, and DHI
    clearsky = panel.site.get_clearsky(times)

    # Get solar azimuth and zenith to pass to the transposition function
    solar_position = panel.site.get_solarposition(times)

    # Use the get_total_irradiance function to transpose the GHI to POA
    POA_irradiance = irradiance.get_total_irradiance(
        surface_tilt=panel.tilt,
        surface_azimuth=panel.azimuth,
        dni=clearsky['dni'],
        ghi=clearsky['ghi'],
        dhi=clearsky['dhi'],
        solar_zenith=solar_position['apparent_zenith'],
        solar_azimuth=solar_position['azimuth'])

    # Return DataFrame with only GHI and POA
    return pd.DataFrame({'GHI': clearsky['ghi'],
                         'POA': POA_irradiance['poa_global']})


def power_plot(panels: List[SolarPanel], temp=20, time=now):
    current_total_power = np.sum(
        [p.get_current_power_output(time) for p in panels])
    print(current_total_power)
    avg_temp_eff = np.mean([p.get_temp_efficiency(temp) for p in panels])
    x_labels = ['Total power generated (W)', 'Avg. panel efficiency']
    y_val = [current_total_power, avg_temp_eff]

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

    return plt_fig_to_pil(fig)


# def get_power_stats(panels: List[SolarPanel], temp=20, time=now) -> T.PanelInfoResponse:
#     current_total_power = np.sum(
#         [p.get_current_power_output(time) for p in panels])
#     avg_irradiance = np.mean([p.get_current_irradiance() for p in panels])
#     avg_temp_eff = np.mean([p.get_temp_efficiency(temp) for p in panels])
#     stats = {'current_total_power': current_total_power,
#              'avg_irradiance': avg_irradiance,
#              'avg_temp_eff': avg_temp_eff}
#     return stats


def get_power_stats(panel: SolarPanel, temp=20, time=now) -> T.PanelInfoResponse:
    return T.PanelInfoResponse(currentPower=panel.get_current_power_output(time),
                               currentIrradiance=panel.get_current_irradiance(
                                   time),
                               effeciency=panel.get_temp_efficiency(temp))


if __name__ == '__main__':
    panels = [SPRMax400(25, 180) for _ in range(4)]
    img = power_plot(panels=panels, temp=20, time=now)
    img.show()
