from etherscan import Etherscan
import pandas as pd
from pandas_profiling import ProfileReport
import holoviews as hv
import holoviews.operation.datashader as hd
from holoviews.operation.datashader import rasterize

from bokeh.plotting import show
hv.extension('bokeh','matplotlib')

eth = Etherscan('9QTG5UR41I61VS7TVWSUF3RP2HAXAJQ1KX')

#result = eth.get_internal_txs_by_address(address= '0x7Be8076f4EA4A4AD08075C2508e481d6C946D12b', startblock= 13670030, endblock= 13670030, sort='asc')
#print(result)
df = pd.read_csv("2021_08_opensea.csv", sep=';')


#df = pd.DataFrame(result)
#df = df.dropna(how='all', axis=1)
df['SellerReward(Eth)'].apply(pd.to_numeric)
amounts = pd.DataFrame()
transactions = df.groupby('Hash')['SellerReward(Eth)']
amounts['Total_value'] = transactions.sum()
amounts['Seller_value'] = transactions.max()
amounts['Commission_value'] = transactions.min()
df = pd.merge(df, amounts, on='Hash', how='left')
df.drop_duplicates(subset='Hash', keep='last', inplace=True)
df.drop(['TraceId', 'SellerReward(Eth)'], axis=1, inplace=True)
df.Date = pd.to_datetime(df.Date)
financial = df.drop(['BlockNumber', 'Hash', 'To'], axis=1)
financial.Date = pd.to_datetime(financial.Date)
#daily = financial.groupby(financial.Date.dt.floor('D')).sum()
#daily['Count'] = financial.groupby(financial.Date.dt.floor('D')).count()
hourly = financial.groupby(financial.Date.dt.floor('H')).sum()
hourly['Day'] = hourly.index.day
hourly['Hour'] = hourly.index.hour
#hourly['Count'] = financial.groupby(financial.Date.dt.floor('H')).count()
#combined = pd.merge(hourly, daily, on='Date', how='left').fillna(method='ffill')
#acounts = df.drop(['BlockNumber', 'Hash'], axis=1)
with pd.option_context("display.max_columns", None, 'display.width', None):
    print(hourly)
hour_day = hv.HeatMap(hourly[['Day', 'Hour', 'Total_value']])
show(hv.render(hour_day))
seller_map = rasterize(hv.Scatter(df[['To', 'Seller_value']]), width=10, height=10)
hv.output(backend="bokeh")
hd.datashade(seller_map)
#hv.render(seller_map)


#profile = ProfileReport(financial, title="Pandas Profiling Report", explorative=True)
#profile.to_file("opensea_report.html")
