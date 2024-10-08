using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using UnityEngine;
using System.Diagnostics;
using System.Text;

public class UDPManager : MonoBehaviour
{
    class UDPData
    {
        private readonly UdpClient udpClient;
        //�ⲿ������
        public UdpClient UDPClient
        {
            get { return udpClient; }
        }

        private readonly IPEndPoint endPoint;
        public IPEndPoint EndPoint
        {
            get { return endPoint; }
        }

        //���캯��
        public UDPData(IPEndPoint endPoint, UdpClient udpClient)
        {
            this.endPoint = endPoint;
            this.udpClient = udpClient;
        }
    }
    public string receiveData = string.Empty;
    private Action<string> ReceiveCallBack = null;
    private Thread RecviveThread;
    void Start()
    {
        ThreadRecvive();
    }
    void Update()
    {
        if (ReceiveCallBack != null && !string.IsNullOrEmpty(receiveData))
        {
            //���ô�����ȥ���ݽ��д���
            ReceiveCallBack(receiveData);
            //ʹ��֮��������յ�����
            receiveData = string.Empty;
        }
    }
    private void OnDestroy()
    {
        if (RecviveThread != null)
        {
            RecviveThread.Abort();
        }
    }
    public void SetReceiveCallBack(Action<string> action)
    {
        ReceiveCallBack = action;
    }
    ///<summary>
    ///��ʼ�߳̽���
    ///</summary>
    private void ThreadRecvive()
    {
        RecviveThread = new Thread(() =>
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 2335);
            UdpClient udpReceive = new UdpClient(endPoint);
            UDPData data = new UDPData(endPoint, udpReceive);
            //�����첽����
            udpReceive.BeginReceive(CallBackRecvive, data);
        })
        {
            IsBackground = true
        };
        //�����߳�
        RecviveThread.Start();
    }

    ///<summary>
    ///�첽���ջص�
    ///</summary>
    ///<param name="ar"></param>
    private void CallBackRecvive(IAsyncResult ar)
    {
        try
        {
            //�����������첽���תΪ������Ҫ����������
            UDPData state = ar.AsyncState as UDPData;
            IPEndPoint ipEndPoint = state.EndPoint;
            //�����첽���� �������ᵼ���ظ������߳̿���
            byte[] data = state.UDPClient.EndReceive(ar, ref ipEndPoint);
            //�������� �����Լ������ݶ�ΪĬ�� ���ͻ��˴������ı������
            receiveData = Encoding.Default.GetString(data);
            //���ݵĽ�����Update��ִ��Unity��Thread�޷��������̵߳ķ���
            //�ٴο����첽��������
            state.UDPClient.BeginReceive(CallBackRecvive, state);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
            throw;
        }
    }
    ///<summary>
    ///����UDP��Ϣ
    /// </summary>
    public void UDPSendMessage(string remoteIP, int remotePort, string message)
    {
        byte[] sendbytes = Encoding.Unicode.GetBytes(message);
        IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);
        UdpClient udpSend = new UdpClient();
        udpSend.Send(sendbytes, sendbytes.Length, remoteIPEndPoint);
        udpSend.Close();
    }
}
