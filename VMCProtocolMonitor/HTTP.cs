using System;
using System.Threading;
using System.IO;
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
        string adr = "";
        string responseBody = "{}";
        public void SetResponse(string res) {
            responseBody = res;
        }

        public HTTP(string adr)
        {
            this.adr = adr;
            listener = new HttpListener();
            listener.Prefixes.Add(adr);

            Console.WriteLine("### View server started on "+adr);
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

                Console.WriteLine(request.Url.LocalPath);

                HttpListenerResponse response = context.Response;
                string res = "";

                try
                {
                    switch (request.Url.LocalPath) {
                        case "/":
                            res = File.ReadAllText("index.htm", new UTF8Encoding(false));
                            break;
                        case "/list.dat":
                            res = responseBody;
                            break;
                        case "/script.js":
                            res = File.ReadAllText("script.js", new UTF8Encoding(false));
                            break;
                        case "/style.css":
                            res = File.ReadAllText("style.css", new UTF8Encoding(false));
                            break;
                        default:
                        res = "404 Not found";
                        response.StatusCode = 404;
                        break;
                    }
                }
                catch (Exception e)
                {
                    response.StatusCode = 500;
                    res = "Internal Server Error";
                    Console.WriteLine(e);
                }

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
