import pandas as pd
import numpy as np
from sklearn.cluster import KMeans
from sklearn.preprocessing import StandardScaler
import matplotlib.pyplot as plt
from sklearn.svm import SVC
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
import seaborn as sns
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
kmeans = KMeans(n_clusters=4, random_state=0)
labels = kmeans.fit_predict(scaled_data)

# 클러스터링 결과를 데이터프레임에 추가
data['Cluster'] = labels

# 라벨 할당
cluster_to_label = {
    0: 'Evasion Prefer',
    1: 'Agressive Prefer',
    2: 'Kill Prefer',
    3: 'Exp Prefer'
}
data['Label'] = data['Cluster'].map(cluster_to_label)

# 모델 저장
model_file_path = "/Users/mylaptop/Documents/Study/Programming/Unity/Vamm_ML/Assets/Resources/My_K_Means_Model(SVM).pkl"
scaler_file_path = "/Users/mylaptop/Documents/Study/Programming/Unity/Vamm_ML/Assets/Resources/My_Scaler(SVM).pkl"
dump(kmeans, model_file_path)
dump(scaler, scaler_file_path)
print(f"Model and scaler saved to {model_file_path} and {scaler_file_path}")

# 군집 결과 시각화
plt.figure(figsize=(10, 8))
scatter = plt.scatter(scaled_data[:, 0], scaled_data[:, 1], c=labels, cmap='viridis', marker='o')
plt.scatter(kmeans.cluster_centers_[:, 0], kmeans.cluster_centers_[:, 1], s=300, c='red', marker='x')

# 각 군집 중심에 라벨 표시
for i, center in enumerate(kmeans.cluster_centers_):
    plt.annotate(cluster_to_label[i], (center[0], center[1]), fontsize=12, fontweight='bold', color='black', backgroundcolor='white')

plt.title('K-Means Clustering of Game Data')
plt.xlabel('Scaled Exp Count')
plt.ylabel('Scaled Kill Count')

# 군집별로 색상 표시
legend1 = plt.legend(*scatter.legend_elements(), title="Clusters")
plt.gca().add_artist(legend1)

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

# 학습을 위한 데이터 준비
X = data[['Exp Count', 'Kill']]
y = data['Label']
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# SVM 모델을 사용한 지도 학습
svm_model = SVC(kernel='linear', random_state=42)
svm_model.fit(X_train, y_train)

# 모델 평가
y_pred = svm_model.predict(X_test)
print(classification_report(y_test, y_pred))

# 예시
# 기존 데이터 중 하나를 선택하여 예측
sample_data = data.sample(n=1, random_state=42)  # 기존 데이터에서 하나를 샘플링
sample_features = sample_data[['Exp Count', 'Kill']]
sample_true_label = sample_data['Label'].values[0]

# 예측 수행
sample_prediction = svm_model.predict(sample_features)[0]

# 예측 결과 출력
print(f"True Label: {sample_true_label}, Predicted Label: {sample_prediction}")

# 결과 시각화
plt.figure(figsize=(10, 6))
sns.scatterplot(x='Exp Count', y='Kill', hue='Label', data=data, palette='viridis', alpha=0.5)
sns.scatterplot(x='Exp Count', y='Kill', data=sample_data, color='red', marker='X', s=100, label=f'Predicted: {sample_prediction}')
plt.title('Sample Prediction on Existing Data')
plt.legend()
plt.show()
