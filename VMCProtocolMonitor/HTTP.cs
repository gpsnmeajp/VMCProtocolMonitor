using System;
using System.Threading;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMCProtocolMonitor
{
    class HTTP : IDisposable
    {
        HttpListener listener;
        Thread thread = null;
        string responseBody = "{}";
        public void SetResponse(string res) {
            responseBody = res;
        }

        public HTTP()
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:8888/");
            listener.Start();

            //受信処理スレッド
            thread = new Thread(new ThreadStart(ReceiveThread));
            thread.Start();

        }
        private void ReceiveThread()
        {
            while (listener.IsListening) {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;

                Console.WriteLine(request.Url);

                HttpListenerResponse response = context.Response;
                string res = responseBody;
                byte[] buf = new UTF8Encoding(false).GetBytes(res);
                response.OutputStream.Write(buf, 0, buf.Length);
                response.OutputStream.Close();

                Thread.Sleep(30);
            }
        }

        public void Dispose()
        {
            listener.Close();
        }
    }
}
