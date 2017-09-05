
///////////////////////////////////////////////////////////////////////////////////
// ICommunicator.cs - Peer-To-Peer Communicator Service Contract                 //
// ver 1.0                                                                       //
// Application: CSE681 - Software Modelling and Analysis, Project #4             //
// Author:      Pranjul Arora, Syracuse University,                              //
//              parora01@syr.edu, (315) 949-9061                                 //
///////////////////////////////////////////////////////////////////////////////////
/*
 * 
 * 
 * Module Operations:
 * ==================
 * Interfaces defined for file streaming and message communication channel
 * defines contracts as well.
 * 
 *  Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 client2
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.ServiceModel;
using MessageCreate;

namespace ClientCommunication // used by all 3, clients, Test Harness server and repository server
{
        // these are for file transfer as defined in Project -> FileStreaming -> IStreamService.cs
        // interfaces and contracts defined
    [ServiceContract]
    public interface IStreamService
    {
        [OperationContract(IsOneWay = true)]
        void upLoadFile(FileTransferMessage msg);
    }

    [MessageContract]
    public class FileTransferMessage
    {
        [MessageHeader(MustUnderstand =true)]
        public string fileName { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream transferStream { get; set; }
    }


    ///////////////////////////////////////    File Transfer end

    [ServiceContract]
    public interface ICommunicator
    {
        [OperationContract(IsOneWay = true)]
        void PostMessage(Messages msg);

        Messages GetMessage();
    }

}
