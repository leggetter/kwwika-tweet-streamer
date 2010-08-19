using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace KwwikaTweetStreamerPublisher
{
    class Program
    {
        private static Publisher _publisher;

        static void Main(string[] args)
        {
            KwwikaTweetStreamerPublisherConfig config = ConfigurationManager.GetSection("KwwikaTweetStreamerPublisherConfig") as KwwikaTweetStreamerPublisherConfig;

            if (config == null)
            {
                throw new ConfigurationErrorsException("The <KwwikaTweetStreamerPublisherConfig> section was either not present or could not be deserialised");
            }
            
            _publisher = new Publisher(config);

            Console.WriteLine("Type \"START\" to start the publisher and \"STOP\" to stop it");

            bool running = true;
            while (running == true)
            {
                string line = Console.ReadLine();
                switch (line.Trim().ToUpper())
                {
                    case "STOP":
                        if (_publisher.Connected == true)
                        {
                            _publisher.Disconnect();
                        }
                        else
                        {
                            Console.Error.WriteLine("STOP command recieved when not connected");
                        }
                        break;
                    case "START":
                        if (_publisher.Connected == false)
                        {
                            _publisher.Connect();
                        }
                        else
                        {
                            Console.Error.WriteLine("START command recieved when already connected");
                        }
                        break;
                }

            }
        }

        private static string GetParameter(string[] paramIdentifiers,string[] args)
        {
            string argument = null;
            string value = null;
            for (int i = 0; i < args.Length; i++)
            {
                argument = args[i];
                foreach (string paramToLookFor in paramIdentifiers)
                {
                    if (argument.ToUpper() == paramToLookFor.ToUpper() &&
                       (i + 1) < args.Length)
                    {
                        value = args[i + 1];
                        Console.WriteLine("Parameter found: " + argument + "=" + value);
                        return value;
                    }
                }
            }
            return null;       
        }
    }
}
