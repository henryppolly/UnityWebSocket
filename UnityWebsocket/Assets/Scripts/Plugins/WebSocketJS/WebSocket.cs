using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.InteropServices;
using System;

namespace WebSocketJS
{
    /// <summary>
    /// <para>WebSocket is a network connection.</para>
    /// <para>It can be connecting,connected,closing or closed state. </para>
    /// <para>You can send and receive messages by using it.</para>
    /// <para>Regist receive callback for handling received messages.</para>
    /// <para>WebSocket 表示一个网络连接，</para>
    /// <para>它可以是 connecting connected closing closed 状态，</para>
    /// <para>可以发送和接收消息，</para>
    /// <para>接收消息处理的地方注册消息回调即可。</para>
    /// </summary>
    public class WebSocket
    {
        public string address { get; private set; }
        public State state { get; private set; }
        public Action onOpen { get; set; }
        public Action onClose { get; set; }
        public Action<byte[]> onReceive { get; set; }
        private WebSocket() { }

        public WebSocket(string address)
        {
            if (WebSocketReceiver.instance == null)
            {
                WebSocketReceiver.AutoCreateInstance();
            }
            this.address = address;
            this.state = State.Closed;
        }

        /*------------- call jslib method --------*/
        [DllImport("__Internal")]
        private static extern void ConnectJS(string str);
        [DllImport("__Internal")]
        private static extern void SendJS(byte[] data, int length);
        [DllImport("__Internal")]
        private static extern void CloseJS();
        [DllImport("__Internal")]
        private static extern void AlertJS(string str);

        public void Connect()
        {
            WebSocketReceiver.instance.AddListener(address, OnOpen, OnClose, OnReceive);
            ConnectJS(address);
            this.state = State.Connecting;
        }

        public void Send(byte[] data)
        {
            SendJS(data, data.Length);
        }

        public void Close()
        {
            CloseJS();
            this.state = State.Closing;
        }

        public void Alert(string str)
        {
            AlertJS(str);
        }

        private void OnOpen()
        {
            if (onOpen != null)
                onOpen.Invoke();
            this.state = State.Connected;
        }

        private void OnReceive(byte[] msg)
        {
            if (onReceive != null)
                onReceive.Invoke(msg);
        }

        private void OnClose()
        {
            if (onClose != null)
                onClose.Invoke();
            this.state = State.Closed;
            WebSocketReceiver.instance.RemoveListener(address);
        }


        public enum State
        {
            Closed,
            Connecting,
            Connected,
            Closing,
        }
    }
}