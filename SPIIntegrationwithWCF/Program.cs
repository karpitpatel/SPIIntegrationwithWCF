using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPIIntegrationwithWCF.MIS;
using System.Configuration;
using System.Threading;

namespace SPIIntegrationwithWCF
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var objProgram = new Program();
            var EftPosAddress = ConfigurationSettings.AppSettings["EFTPOSAddress"];
            var PosId = ConfigurationSettings.AppSettings["POSId"];
            using (var serviceClient = new IntegrationServiceSoapClient())
            {
                var pairingDevcieResponse = serviceClient.StartPairing(objProgram.GetAuthenticationHeader(), new PairingRequest()
                {
                    ClientId = "50055",
                    EftPosAddress = EftPosAddress,
                    PosId = PosId,
                    TerminalId = "1"
                });
                if (pairingDevcieResponse.Status == OperationStatus.Success)
                {
                    Thread.Sleep(2000);
                    var confirmPairingDeviceResponse = serviceClient.ConfirmPairing(objProgram.GetAuthenticationHeader(), new ConfirmPairingRequest()
                    {
                        ClientId = "50055",
                        ConfirmationCode = pairingDevcieResponse.Message,
                        EftPosAddress = EftPosAddress,
                        PosId = PosId,
                        TerminalId = "1"
                    });

                    if (confirmPairingDeviceResponse.Status == OperationStatus.Success)
                    {
                        Thread.Sleep(3000);
                        var makePaymentRespnse = serviceClient.MakePayment(objProgram.GetAuthenticationHeader(), new PaymentRequest()
                        {
                            Amount= 10,
                            ClientId = "50055",
                            EftPosAddress = EftPosAddress,
                            PosId = PosId,
                            TerminalId ="1",
                            TipAmount=0
                        });

                        if (makePaymentRespnse.Status == OperationStatus.Success)
                            Console.WriteLine(makePaymentRespnse.CustomerReceipt);
                        else
                            Console.WriteLine(makePaymentRespnse.Error);

                    }
                }
                Console.ReadLine();
            }
        }

        protected AuthenticationHeader GetAuthenticationHeader()
        {
            return new AuthenticationHeader
            {
                Signature = "Micropower Member Services",
                EncryptedIdentity = null
            };
        }
    }
}
