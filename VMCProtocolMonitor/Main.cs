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
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rug.Osc;
using Newtonsoft.Json;
/*
 * InputJson : 入力ポートと入力名
 * OutputJson : 出力ポートと入力名
 * Filter Json: 振り分け法則(入力名-フィルタ-出力名)
 */

namespace VMCProtocolMonitor
{
    class Main
    {
        const string version = "VMCProtocolMonitor v0.01";
        Setting setting;
        OSC osc;

        SortedDictionary<string, OscMessage> packets = new SortedDictionary<string, OscMessage>();

        public void Process()
        {
            Console.WriteLine("### "+ version);
            //---------サーバー開始------------
            try
            {
                StartServer();

                if (setting.ListMode)
                {
                    ListMode();
                }
                else {
                    StreamMode();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("# MainThread : " + e);
            }

            //---------サーバー停止------------
            try
            {
                StopServer();
            }
            catch (Exception e)
            {
                Console.WriteLine("# MainThread : " + e);
            }

            Console.WriteLine("Press ENTER key to close window");
            Console.ReadLine();
        }

        private void StreamMode()
        {
            Console.WriteLine("Press ENTER key to stop server");
            Console.ReadLine();
        }
        private void ListMode()
        {
            bool exit = false;
            int cnt = 0;
            int oldviewCount = 0;
            HTTP http = new HTTP("http://127.0.0.1:8888/");

            Console.WriteLine("### Press ENTER key to stop server");

            while (!exit)
            {
                string responce = "";

                List<KeyValuePair<string,OscMessage>> view = packets.ToList();
                for (int i = 0; i < view.Count; i++) {
                    responce += ("# "+view[i].Value)+"\n";
                }
                http.SetResponse(responce);

                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                    {
                        exit = true;
                    }
                }

                Thread.Sleep(500);
            }
            http.Dispose();
        }

        private void StartServer()
        {
            setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText("setting.json", new UTF8Encoding(false)));
            Console.WriteLine("# Setting loaded");

            osc = new OSC(setting.Port, OnBundle, OnMessage);
        }
        private void StopServer()
        {
            osc.Dispose();
        }

        private void OnMessage(OscMessage message) {
            if (setting.ListMode)
            {
                packets[MakeKeyOfMessage(message)] = message;
            }
            else {
                Console.WriteLine("## " + message);
            }
        }
        private void OnBundle(OscBundle bundle) {
            if (!setting.ListMode)
            {
                Console.WriteLine("## Bundle begin");
            }
            for (int i = 0; i < bundle.Count; i++) {
                switch (bundle[i])
                {
                    //Messageを受信した
                    case OscMessage msg:
                        if (setting.ListMode)
                        {
                            packets[MakeKeyOfMessage(msg)] = msg;
                        }
                        else {
                            Console.WriteLine("# " + msg);
                        }
                        break;
                    default:
                        //Do noting
                        break;
                }
            }
        }

        private string MakeKeyOfMessage(OscMessage msg) {
            if (msg.Count >= 1)
            {
                //長さが1以上の場合は、1番目の要素も含む(VMCProtocolに基づく)
                //ただし1番目の要素が大きく変動するものに関しては除外する
                //2番目がキーになっているのにに関してはそういう扱いをする
                switch (msg.Address) {
                    //変化除外
                    case "/VMC/Ext/OK":
                    case "/VMC/Ext/T":
                    case "/VMC/Ext/Rcv":
                    case "/VMC/Ext/VRM":
                    case "/VMC/Ext/Opt":
                    case "/VMC/Ext/Setting/Color":
                    case "/VMC/Ext/Setting/Win":
                    case "/VMC/Ext/Config":

                    case "/VMC/Ext/Set/Period":
                    case "/VMC/Ext/Set/Eye":
                    case "/VMC/Ext/Set/Res":
                    case "/VMC/Ext/Set/Calib/Exec":
                    case "/VMC/Ext/Set/Config":
                        return msg.Address;

                    //特殊変化(2番目)
                    case "/VMC/Ext/Con": // //VMC/Ext/Con (int){active} (string){name} (int){IsLeft} (int){IsTouch} (int){IsAxis} (float){Axis.x} (float){Axis.y} (float){Axis.z} 
                    case "/VMC/Ext/Key": // /VMC/Ext/Key (int){active} (string){name} (int){keycode}
                        if (msg.Count >= 2)
                        {
                            return msg.Address + msg[1].ToString();
                        }
                        else
                        {
                            return msg.Address;
                        }
                    //特殊変化(3番目)
                    case "/VMC/Ext/Midi/Note": // /VMC/Ext/Midi/Note (int){active} (int){channel} (int){note} (float){velocity}

                        if (msg.Count >= 3)
                        {
                            return msg.Address + msg[2].ToString();
                        }
                        else
                        {
                            return msg.Address;
                        }

                    default:
                        return msg.Address + msg[0].ToString();
                }
            }
            else {
                //長さ0の場合はアドレスのみ
                return msg.Address;
            }
        }
    }
}
