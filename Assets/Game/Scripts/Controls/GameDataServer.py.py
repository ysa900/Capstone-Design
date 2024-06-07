import socket
import pickle
import numpy as np
from sklearn.externals import joblib

# 모델 파일 경로
model_file_path = "kmeans_model.pkl"
scaler_file_path = "scaler.pkl"

# 학습된 모델 로드
kmeans = joblib.load(model_file_path)
scaler = joblib.load(scaler_file_path)
print("Model and scaler loaded from", model_file_path, "and", scaler_file_path)

# 서버 소켓 설정
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(('localhost', 12345))
server_socket.listen(1)

print("Waiting for a connection...")
connection, client_address = server_socket.accept()

try:
    print(f"Connection from {client_address}")

    while True:
        # 데이터 수신
        data = connection.recv(4096)
        if not data:
            break
        data_list = pickle.loads(data)
        print(f"Received data: {data_list}")

        # 데이터를 배열 형태로 변환하여 표준화
        player_data = np.array([data_list])
        scaled_data = scaler.transform(player_data)

        # 군집화 수행
        cluster_label = kmeans.predict(scaled_data)[0]

        # 결과 전송
        data_to_send = pickle.dumps(cluster_label)
        connection.sendall(data_to_send)
        print(f"Sent cluster index: {cluster_label}")
finally:
    connection.close()
    server_socket.close()