import disruptive as dt
import matplotlib.pyplot as plt
from datetime import datetime, timedelta

project_id = 'c1qprqlh2t8g02sfoq30'
dt.default_auth = dt.Auth.service_account('C73lmqav1410008ofpvg', 'a97dd7320f7845b7b6c7e2499f9e03c9', 'c73llpqv1410008ofpug@c1qprqlh2t8g02sfoq30.serviceaccount.d21s.com')

# #CloudConnector=blvjthsc0001nr3n4nt0

sensor = dt.Device.get_device('bjmghou7kro000cp4gng')


device_ids = ['bjihp8m7kro000cp1tug', 'bu6oa4uptglg00ehc5b0', 'bjmd4i5p0jt000a5cldg', 'bu6onheptglg00ehcfag', 'bjihev8pismg008hrkig', 'bv8t0aqvje9g00eg5ma0', 'bjqhdclntbig00f92te0', 'bjqheb5ntbig00f92tng', 'bu6oqm988ueg00albpbg', 'bu6o15p88ueg00alb6c0', 'bjihd467gpvg00cjpb1g', 'btmsltc9kjug00dlsibg', 'bjiier0pismg008hru40', 'btmu2i5pna500081fju0', 'bjijkbe7kro000cp2feg', 'btou87tpna500081gi20',
              #            'bv8stgs2ven000ch8k2g','bjihanlntbig00e44lc0','bu6nh7h88ueg00alaq2g','bjej76u7gpvg00cjoe20',#            'bjejt50pismg008hqtf0','bjekggtp0jt000aqcsug','bjihmfe7kro000cp1t5g','bjii1qlntbig00e44s50',#            'bjii8c7bluqg00dlv2gg','bjm1ft7bluqg00dlvrng','bjm2u6opismg008ht2a0','bjmgat7bluqg00dm1dt0',#            'bjmgdle7gpvg00cjrvqg','bjmghou7kro000cp4gng','bjqhan67kro000cp7ojg','bjqhc85p0jt000a5gkg0',#            'bt3nmnj853cg00fllacg'           
              ]

for devices in device_ids:
    sensor = dt.Device.get_device(devices)
    print(sensor.display_name)


plt.figure(figsize=(15, 7))
for devices in device_ids:
    # Fetch temperature events for the last 2 days.
    event_history = dt.EventHistory.list_events(
        device_id=devices, project_id=project_id,
        event_types=[dt.events.TEMPERATURE],
        start_time=datetime.today()-timedelta(days=25),)
    # Isolate timeaxis and temperature data which can be plotted directly.
    timestamps = [event.data.timestamp for event in event_history]
    temperature = [event.data.celsius for event in event_history]
    # Generate a plot using the fetched timeaxis and temperature values.
    sensor_name = dt.Device.get_device(devices).display_name
    plt.plot(timestamps, temperature, '.', label=sensor_name)
    plt.legend()
plt.grid()
plt.xlabel('Timestamp')
plt.ylabel('Temperature [C]')
plt.show()
