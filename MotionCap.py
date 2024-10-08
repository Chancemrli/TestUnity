import time

import cv2
import numpy
from cvzone.PoseModule import PoseDetector
import socket
import numpy as np


 """def SendImageToUnity(image, client, encode_param):
    result, imgencode = cv2.imencode('.jpg', image, encode_param)
    # 建立矩阵
    data = numpy.array(imgencode)
    # 将numpy矩阵转换成字符形式，以便在网络中传输
    stringData = data.tobytes()

    # 先发送要发送的数据长度
    print(len(stringData))
    strlen = str(len(stringData)).ljust(16)
    client.send(bytes(strlen, 'UTF-8'))
    client.send(stringData)"""


def main():
    # cap = cv2.VideoCapture('WeChat_20230218110458.mp4')
    cap = cv2.VideoCapture(0)
    # cap = cv2.VideoCapture("rockstar.mp4")
    # 套接字
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    Port = 2335
    LocalHost = '127.0.0.1'
    ServerAddr = (LocalHost, Port)

    # client = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    # client_port = 2776

    detector = PoseDetector()
    # posList = []    # 保存到txt在unity中读取需要数组列表
    jointnum = 33

    # 平滑
    # kalman filter parameters
    KalmanParamQ = 0.001
    KalmanParamR = 0.0015
    K = np.zeros((jointnum, 3), dtype=np.float32)
    P = np.zeros((jointnum, 3), dtype=np.float32)
    X = np.zeros((jointnum, 3), dtype=np.float32)
    # 低通滤波参数
    PrevPose3D = np.zeros((6, jointnum, 3), dtype=np.float32)

    while True:
        success, img = cap.read()
        # img = cv2.resize(img, (480,860))
        if not success:
            """
            with open("AnimationFile.txt", 'w') as f:
                f.writelines(["%s\n" % item for item in posList])
            """
            break

        """
        connect=False
        try:
            if connect == False:
                client.connect((LocalHost, client_port))
                connect = True

            encode_param=[int(cv2.IMWRITE_JPEG_QUALITY),70]
            SendImageToUnity(img,client,encode_param)
        except socket.error as e:
            print("Address-related error connecting to server: %s" % e)
            connect=False
        """

        img = detector.findPose(img)
        lmList, bboxInfo = detector.findPosition(img)
        if bboxInfo:
            lmString = ''
            currdata = np.squeeze(lmList)
            print((currdata))
            smooth_kps = np.zeros((jointnum, 3), dtype=np.float32)
            '''
            33个关键点的卡尔曼滤波器
            '''
            for i in range(jointnum):
                K[i] = (P[i] + KalmanParamQ) / (P[i] + KalmanParamQ + KalmanParamR)
                P[i] = KalmanParamR * (P[i] + KalmanParamQ) / (P[i] + KalmanParamQ + KalmanParamR)
            for i in range(jointnum):
                smooth_kps[i] = X[i] + (currdata[i] - X[i]) * K[i]
                X[i] = smooth_kps[i]

            # datakalman[idx] = smooth_kps # record kalman result

            '''
            low pass filter
            '''
            LowPassParam = 0.1
            PrevPose3D[0] = smooth_kps
            for j in range(1, 6):
                PrevPose3D[j] = PrevPose3D[j] * LowPassParam + PrevPose3D[j - 1] * (1.0 - LowPassParam)
            # datafinal[idx] = PrevPose3D[5] # record kalman+low pass result.
            print(PrevPose3D[5])
            for lm in PrevPose3D[5]:
                print(lm)
                lmString += f'{lm[0]},{img.shape[0] - lm[1]},{lm[2]},'
            # posList.append(lmString)
            if lmString:
                date = lmString
                sock.sendto(date.encode(), ServerAddr)

        cv2.namedWindow("Image", 0)
        cv2.resizeWindow("Image", 360, 640);
        cv2.imshow("Image", img)
        key = cv2.waitKey(1)
        # 记录数据到本地
        # if key == ord('r'):
        # with open("MotionData.txt", 'w') as f:
        #     f.writelines(["%s\n" % item for item in posList])
        if key == ord('q'):
            """
            with open("AnimationFile.txt",'w') as f:
                f.writelines(["%s\n" % item for item in posList])
            """
            break
    sock.close()
    # client.close()
    cap.release()
    cv2.destroyAllWindows()

if __name__=="__main__":
    main()