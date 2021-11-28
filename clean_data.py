from etherscan import Etherscan
import pandas as pd
from pandas_profiling import ProfileReport

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

with pd.option_context("display.max_columns", None, 'display.width', None):
    print(df)

profile = ProfileReport(df, title="Pandas Profiling Report", explorative=True)
profile.to_file("opensea_report.html")
