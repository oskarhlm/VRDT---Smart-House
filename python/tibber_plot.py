import tibber
import numpy as np
import plotly.graph_objects as go
import pandas as pd
from plotly.subplots import make_subplots
import io
from PIL import Image
from base_pb2 import TibberMessages as T


def tibber_realtime(time_resolution=T.TimeResolution, time_units=7, filename="tibber", save_img=False):
    account = tibber.Account("hoMAFvQcasnMQyhIZpprgcKbdhOEYen3PBcpmX0q_K4")
    home = account.homes[0]

    match time_resolution:
        case T.TimeResolution.HOUR:
            resolutions_str = 'hour'
            data = home.fetch_consumption("HOURLY", last=time_units)
        case T.TimeResolution.DAY:
            resolutions_str = 'day'
            data = home.fetch_consumption("DAILY", last=time_units)
        case T.TimeResolution.MONTH:
            resolutions_str = 'month'
            data = home.fetch_consumption("MONTHLY", last=time_units)
        case _:
            raise "Time resultion value not recognized"

    energy = []
    cost = []
    price = []
    for d in data:
        energy.append(d.consumption)
        cost.append(d.cost),
        price.append(d.unit_price)

    energy = nonetype_to_zero(energy)
    cost = nonetype_to_zero(cost)
    price = nonetype_to_zero(price)

    num = np.linspace(0, len(energy), len(energy))
    data_dic = {"Energy kWh": energy,
                "Num": num,
                "Cost/": cost,
                "Unit price/": price}
    df = pd.DataFrame(data_dic)
    fig_sub = make_subplots(rows=2, cols=1,
                            subplot_titles=["Peak electricity price/" + resolutions_str, "Consumption kWh"])  # rows=2, cols=2)

    fig_sub.add_trace(
        go.Bar(x=df["Num"], y=df["Energy kWh"], marker=dict(
            color=df["Cost/"], coloraxis="coloraxis")),
        row=2, col=1)

    fig_sub.add_trace(
        go.Scatter(x=df["Num"], y=df["Unit price/"],
                   marker=dict(color=df["Unit price/"], coloraxis="coloraxis")),
        row=1, col=1)

    fig_sub['layout']['xaxis']['title'] = resolutions_str + 's'
    fig_sub['layout']['xaxis2']['title'] = resolutions_str + 's'
    fig_sub['layout']['yaxis']['title'] = 'Kr/kWh'
    fig_sub['layout']['yaxis2']['title'] = 'kWh'

    title_txt = "Energy Consumption over {days:.0f} " + \
        resolutions_str + "s"

    fig_sub.update_layout(title=title_txt.format(days=len(df)),
                          title_font_size=30,
                          coloraxis_colorbar=dict(
                              title="Cost/" + resolutions_str),
                          coloraxis=dict(colorscale='Inferno'),
                          showlegend=False)

    filename = "img/" + filename + ".png"
    if save_img:
        fig_sub.write_image(filename)

    image_bytes = fig_sub.to_image(format='png')
    pil_image = Image.open(io.BytesIO(image_bytes))
    return pil_image


def nonetype_to_zero(list):
    for i in range(len(list)):
        if list[i] is None:
            list[i] = 0.0
    return list


# tibber_realtime(time_resolution="hour", time_units=24, filename="last24h")
# tibber_realtime(time_resolution="day",  time_units=7, filename="lastweek")
# tibber_realtime(time_resolution="day", time_units=30, filename="lastmonth")
# tibber_realtime(time_resolution="month", time_units=7, filename="lastyear")
