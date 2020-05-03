/*
MIT License

Copyright (c) 2020 gpsnmeajp

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
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

                //Console.WriteLine(request.Url.LocalPath);

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
                        case "/worker.js":
                            res = File.ReadAllText("worker.js", new UTF8Encoding(false));
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
