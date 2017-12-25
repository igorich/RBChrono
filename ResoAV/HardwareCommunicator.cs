using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ResoAV
{
	static class HardwareCommunicator
	{
        public delegate void ComEventHandler(byte type, object args);
        //static public event ComEventHandler ConnectionStatus;
        static public event ComEventHandler ReceivedData;

        private static System.IO.Ports.SerialPort _com;

        static HardwareCommunicator()
        {
            Initialize();
        }

        private static void Initialize()
        {
            string port = RaceSettings.RS.CommunicationPort;
            System.ComponentModel.IContainer components = new System.ComponentModel.Container();
            
            _com = new System.IO.Ports.SerialPort(components); // Creating the new object.
            try
            {
                _com.PortName = port; // Setting what port number.
                _com.BaudRate = 9600; // Setting baudrate.
                _com.DtrEnable = true; // Enable the Data Terminal Ready 
                _com.Open(); // Open the port for use.
                _com.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(DataReceived);
                //ConnectionStatus(0, null);
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message); 
            }
        }

        public static void ReInitialize()
        {
            _com.Close();
            _com.Dispose();
            Initialize();
        }

        static void DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string all = _com.ReadExisting();
            var l = all.Last();
            byte command = (byte)l;
            Console.WriteLine(String.Format("com: command - {0}, full string - {1}*",command, all));
            ReceivedData(command, null);
            //throw new NotImplementedException();
        }

        public static void Close()
        {
            _com.Close();
            _com.Dispose();
        }

		static public void RunCountdown(ResultCallbackDelegate callback)
		{
			var countdown = new Thread(_RunCountdown);
			countdown.SetApartmentState(ApartmentState.STA);
			countdown.Start(callback);
		}

		public delegate void ResultCallbackDelegate(int state);
        static private void _RunCountdown(object o)
		{
			var callback = o as ResultCallbackDelegate;
			if(callback == null)
				return;
			var pkt = new ResoPacket();
			pkt.Command = 5;
			callback(5);
            for (int i = 5; i >= -1; i--)
			{
				pkt.Command = (byte)i;
				callback(i);
                if(_com.IsOpen)
                    _com.Write(new byte[] {(byte)i}, 0, 1);
				Thread.Sleep(1000);
			}

            callback(-1);
		}

    }
    public class ResoPacket
	{
		public byte Command
		{ set; get; }
		private byte[] _data =  { 0x1D , 0x00, 0x00, 0xD1};
		public byte[] Data()
		{
			_data[1] = Command;
			_data[2] = (byte)(_data[1] ^ 0xFF);//TODO проверить ксорит ли оно
			return _data;
            //return {};
		}
	}
}
