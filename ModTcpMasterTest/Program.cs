using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Modbus.Device;

/// <summary>
/// Summary description for Class1
/// </summary>
public class TestDriver
{

    static void Main(string[] args)
    {
        try
        {
            using (TcpClient client = new TcpClient("192.168.0.106", 502))
            {
                client.ReceiveTimeout = (int)TimeSpan.FromSeconds(1).TotalMilliseconds;    
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                Console.WriteLine("{0}", client.Client.SocketType);
                Console.WriteLine("{0}", client.Client.Blocking);
                
                ModbusTcpMaster master = ModbusTcpMaster.CreateTcp(client);
                // read five input values
                ushort startAddress = 0x10;
                ushort numInputs = 10;
                client.Client.Blocking = true;
                ushort[] inputs;//  = master.ReadHoldingRegisters(startAddress, numInputs);
                string  readVal = "";
                byte slaveAddr = 2;
                UInt64 mergedVal64 = 0;
                UInt32 mergedVal32 = 0;
                Int64 mergedValI64 = 0;
                Int32 mergedValI32 = 0;

                while (true)
                {
                    //if (Console.KeyAvailable)
                    {
                        try{
                            Console.WriteLine(" Start Address 입력: ");
                            readVal = Console.ReadLine();
                            startAddress = ushort.Parse(readVal);
                            Console.WriteLine(" num inputs 입력: ");
                            readVal = Console.ReadLine();
                            numInputs = ushort.Parse(readVal);
                        }
                        catch{
                            Console.WriteLine("잘못된 입력.");
                            Console.ReadKey();
                        }

                    }
                    Thread.Sleep(500);
                    Console.Clear();

                    inputs = master.ReadHoldingRegisters(slaveAddr, startAddress, numInputs);
                    for (int i = 0; i < numInputs; i++)
                        Console.WriteLine("Input {0}={1}", startAddress + i, inputs[i]);

                    Console.WriteLine("Merged value -> ");
                    //merge-----------------------
                    switch (numInputs)
                    {
                        case 2: //32bit. 2 register
                            mergedVal32 = 0;
                            mergedValI32 = 0;
                            mergedVal32 = (UInt32)(inputs[0]&0xFFFF) << 16;
                            mergedVal32 |= (UInt32)(inputs[1]&0xFFFF);
                            mergedValI32 = (Int32)mergedVal32;
                            Console.WriteLine("UInt32: {0}", mergedVal32);
                            Console.WriteLine("Int32: {0}", mergedValI32);

                            break;
                        case 4: //64bit
                            mergedVal64 = 0;
                            mergedValI64 = 0;
                            mergedVal64 =  (UInt64)(inputs[0] & 0xFFFF) << 48;
                            mergedVal64 |= (UInt64)((UInt64)inputs[1] & (UInt64)0xFFFF) << 32;
                            mergedVal64 |= (UInt64)((UInt64)inputs[2] & (UInt64)0xFFFF) << 16;
                            mergedVal64 |= (UInt64)((UInt64)inputs[3] & (UInt64)0xFFFF);
                            mergedValI64 = (Int64)mergedVal64;
                            Console.WriteLine("UInt64: {0}", mergedVal64);
                            Console.WriteLine("Int64: {0}", mergedValI64);
                            break;
                    }//--------------------------
                }

               
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.ReadKey();
        }
    }
}
