import pandas as pd
import numpy as np
from sklearn.cluster import KMeans
from sklearn.preprocessing import StandardScaler
import matplotlib.pyplot as plt
from joblib import dump

# CSV 파일 경로
csv_file_path = "/Users/mylaptop/Documents/Study/Programming/Unity/Vamm_ML/Assets/Resources/RefinedMLplayerInfo_Final.csv"

# CSV 파일 읽기
data = pd.read_csv(csv_file_path, na_values=['#DIV/0!'])
# NaN 값을 포함한 행 삭제
data = data.dropna(subset=['Exp Count', 'Kill'])

# 'Exp Count'와 'Kill' 데이터 추출
exp_counts = data['Exp Count'].values
kill_counts = data['Kill'].values

# 데이터 전처리 (표준화)
scaler = StandardScaler()
scaled_data = scaler.fit_transform(np.column_stack((exp_counts, kill_counts)))

# K-Means Clustering
kmeans = KMeans(n_clusters=3, random_state=0)
labels = kmeans.fit_predict(scaled_data)

# 클러스터링 결과를 데이터프레임에 추가
data['Cluster'] = labels

# 모델 저장
model_file_path = "/Users/mylaptop/Documents/Study/Programming/Unity/Vamm_ML/Assets/Resources/My_K_Means_Model.pkl"
scaler_file_path = "/Users/mylaptop/Documents/Study/Programming/Unity/Vamm_ML/Assets/Resources/My_Scaler.pkl"
dump(kmeans, model_file_path)
dump(scaler, scaler_file_path)
print(f"Model and scaler saved to {model_file_path} and {scaler_file_path}")

# 군집 결과 시각화
plt.figure(figsize=(10, 8))
plt.scatter(scaled_data[:, 0], scaled_data[:, 1], c=labels, cmap='viridis', marker='o')
plt.scatter(kmeans.cluster_centers_[:, 0], kmeans.cluster_centers_[:, 1], s=300, c='red', marker='x')
plt.title('K-Means Clustering of Game Data')
plt.xlabel('Scaled Exp Count')
plt.ylabel('Scaled Kill Count')
plt.show()

# 각 군집의 중심 출력
print("Cluster Centers:")
print(scaler.inverse_transform(kmeans.cluster_centers_))

# 결과 CSV 파일 저장 (원본 데이터에 클러스터 라벨 추가)
output_csv_file_path = "/Users/mylaptop/Documents/Study/Programming/Unity/Vamm_ML/Assets/Resources/playerInfo_with_clusters.csv"
data.to_csv(output_csv_file_path, index=False)

# 각 군집의 데이터 개수 출력
print("Cluster Counts:")
print(data['Cluster'].value_counts())