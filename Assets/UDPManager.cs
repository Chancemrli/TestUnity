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
        //外部访问器
        public UdpClient UDPClient
        {
            get { return udpClient; }
        }

        private readonly IPEndPoint endPoint;
        public IPEndPoint EndPoint
        {
            get { return endPoint; }
        }

        //构造函数
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
            //调用处理函数去数据进行处理
            ReceiveCallBack(receiveData);
            //使用之后清理接收的数据
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
    ///开始线程接收
    ///</summary>
    private void ThreadRecvive()
    {
        RecviveThread = new Thread(() =>
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 2335);
            UdpClient udpReceive = new UdpClient(endPoint);
            UDPData data = new UDPData(endPoint, udpReceive);
            //开启异步接收
            udpReceive.BeginReceive(CallBackRecvive, data);
        })
        {
            IsBackground = true
        };
        //开启线程
        RecviveThread.Start();
    }

    ///<summary>
    ///异步接收回调
    ///</summary>
    ///<param name="ar"></param>
    private void CallBackRecvive(IAsyncResult ar)
    {
        try
        {
            //将传过来的异步结果转为我们需要解析的类型
            UDPData state = ar.AsyncState as UDPData;
            IPEndPoint ipEndPoint = state.EndPoint;
            //结束异步接收 不结束会导致重复挂起线程卡死
            byte[] data = state.UDPClient.EndReceive(ar, ref ipEndPoint);
            //解析数据 编码自己调整暂定为默认 依客户端传过来的编码而定
            receiveData = Encoding.Default.GetString(data);
            //数据的解析再Update里执行Unity中Thread无法调用主线程的方法
            //再次开启异步接收数据
            state.UDPClient.BeginReceive(CallBackRecvive, state);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
            throw;
        }
    }
    ///<summary>
    ///发送UDP信息
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
