using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telepexe
{
    class Program
    {
        ~Program()
        {
            
        }

        static void Main(string[] args)
        {
            //ルートディレクトリを決める
            Console.WriteLine("ルートディレクトリを指定してください。");
            string rootPath = Console.ReadLine() + "\\";
                
            //ListenするIPアドレスを決める
            System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse("127.0.0.1");
            
            //Listenするポート番号
            int port = 6900;

            //TcpListenerオブジェクトを作成する
            System.Net.Sockets.TcpListener listener =
                new System.Net.Sockets.TcpListener(ipAdd, port);

            //Listenを開始する
            listener.Start();
            Console.WriteLine("Listenを開始しました({0}:{1})。",
                ((System.Net.IPEndPoint)listener.LocalEndpoint).Address,
                ((System.Net.IPEndPoint)listener.LocalEndpoint).Port);

            System.Net.Sockets.TcpClient client;
            while (true)
            {

                //接続要求があったら受け入れる
                client = listener.AcceptTcpClient();
                Console.WriteLine("クライアント({0}:{1})からの通信を受け取りました。",
                    ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address,
                    ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Port);

                //NetworkStreamを取得
                System.Net.Sockets.NetworkStream ns = client.GetStream();

                //クライアントから送られたデータを受信する
                System.Text.Encoding enc = System.Text.Encoding.UTF8;
                bool disconnected = false;
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                byte[] resBytes = new byte[256];
                do
                {
                    //データの一部を受信する
                    int resSize = ns.Read(resBytes, 0, resBytes.Length);
                    //Readが0を返した時はクライアントが切断したと判断
                    if (resSize == 0)
                    {
                        disconnected = true;
                        Console.WriteLine("受信が完了しました。");
                        break;
                    }
                    //受信したデータを蓄積する
                    ms.Write(resBytes, 0, resSize);
                } while (ns.DataAvailable);
                //受信したデータを文字列に変換
                string resMsg = enc.GetString(ms.ToArray());
                ms.Close();

                //適当にWindows用ファイルパスに整形
                string targetPath = rootPath + resMsg;
                targetPath = targetPath.Replace("/", "\\");
                targetPath = targetPath.Replace("\n", "");
                Console.WriteLine(targetPath + " を開きます。");

                //受け取ったファイルパスを開く
                try
                {
                    System.Diagnostics.Process.Start(targetPath);
                }
                catch (Exception e) // 全部握りつぶす
                {
                    Console.WriteLine(e.Message);
                }

                //閉じる
                ns.Close();
                client.Close();
            }

            //リスナを閉じる
            listener.Stop();
            Console.WriteLine("Listenerを閉じました。");

            Console.ReadLine();
        }
    }
}
