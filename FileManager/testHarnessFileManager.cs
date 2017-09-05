//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO;
//using ClientCommunication;
//using System.ServiceModel;
//using MessageCreate;

//namespace FileManager
//{
//    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
//    public class testHarnessFileManager: IStreamService
//    {
//        string filename;
//        string savePath = @"..\..\..\..\RepositorySavedFiles";
//     //   string ToSendPath = "..\\ToSend";
//        int BlockSize = 1024;
//        byte[] block;
//        HRTimer.HiResTimer hrt = null;

//        public testHarnessFileManager()
//        {
//            block = new byte[BlockSize];
//            hrt = new HRTimer.HiResTimer();
//        }


//        public void upLoadFile(FileTransferMessage msg)
//        {
//            int totalBytes = 0;
//            hrt.Start();
//            filename = msg.fileName;
//            string rfilename = Path.Combine(savePath, filename);
//            if (!Directory.Exists(savePath))
//                Directory.CreateDirectory(savePath);
//            using (var outputStream = new FileStream(rfilename, FileMode.Create))
//            {
//                while (true)
//                {
//                    int bytesRead = msg.transferStream.Read(block, 0, BlockSize);
//                    totalBytes += bytesRead;
//                    if (bytesRead > 0)
//                        outputStream.Write(block, 0, bytesRead);
//                    else
//                        break;
//                }
//            }
//            hrt.Stop();
//            Console.Write(
//              "\n  Received file \"{0}\" of {1} bytes in {2} microsec.",
//              filename, totalBytes, hrt.ElapsedMicroseconds
//            );
//        }









//        static void Main(string[] args)
//        {
//        }
//    }
//}
