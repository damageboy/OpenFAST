using System;
using OpenFAST.Sessions;
using OpenFAST.Template;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;

namespace OpenFAST.TCPServer
{
    public class SessionHandler : ISessionHandler
    {
        #region ISessionHandler Members

        public void NewSession(Session session)
        {
            //session.IsListening = true;
            session.ErrorHandler = new FastServer.ServerErrorHandler();
            session.MessageHandler = new MessageListener();
            //register a template
            ITemplateRegistry registry = new BasicTemplateRegistry();
            var template = new MessageTemplate(
                "Arbitry",
                new Field[]
                    {
                        new Scalar("1", FastType.I32, Operator.Copy, ScalarValue.Undefined, false),
                        new Scalar("2", FastType.I32, Operator.Delta, ScalarValue.Undefined, false),
                        new Scalar("3", FastType.I32, Operator.Increment, new IntegerValue(10), false),
                        new Scalar("4", FastType.I32, Operator.Increment, ScalarValue.Undefined, false),
                        new Scalar("5", FastType.I32, Operator.Constant, new IntegerValue(1), false),
                        /* NON-TRANSFERRABLE */
                        new Scalar("6", FastType.I32, Operator.Default, new IntegerValue(2), false)
                    });
            registry.Register(24, template);

            session.MessageInputStream.RegisterTemplate(24, template);
            //send that template
            session.SendTemplates(registry);

            while (true)
            {
                DateTime startTime = DateTime.Now;
                for (int i = 0; i < 64000; i++)
                {
                    //make a message
                    var message = new Message(template);
                    message.SetInteger(1, 109);
                    message.SetInteger(2, 29470);
                    message.SetInteger(3, 10);
                    message.SetInteger(4, 3);
                    message.SetInteger(5, 1);
                    message.SetInteger(6, 2);

                    //send it to client
                    session.MessageOutputStream.WriteMessage(message);
                }
                double seconds = (DateTime.Now - startTime).TotalSeconds;
                Console.WriteLine(seconds);
                Console.WriteLine("MSG/S:" + (64000/seconds).ToString("0"));
            }
        }

        #endregion
    }
}