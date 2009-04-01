using OpenFAST.Session;
using OpenFAST.Template;
using OpenFAST.Template.Type;
using OpenFAST.Template.Operator;
using OpenFAST;
using System;

namespace TCPServer
{
    public class FASTSessionHandler : SessionHandler
    {
        public void NewSession(Session session)
        {
            //session.Listening = true;
            session.ErrorHandler = new TCPServer.FASTServer.ServerErrorHandler();
            session.MessageHandler = new FASTMessageListener();
            //register a template
            TemplateRegistry registry = new BasicTemplateRegistry();
            var template = new MessageTemplate("Arbitry",
       new Field[] {
                    new Scalar("1", FASTType.I32, Operator.COPY, ScalarValue.UNDEFINED, false),
                    new Scalar("2", FASTType.I32, Operator.DELTA, ScalarValue.UNDEFINED, false),
                    new Scalar("3", FASTType.I32, Operator.INCREMENT,
                        new IntegerValue(10), false),
                    new Scalar("4", FASTType.I32, Operator.INCREMENT,
                        ScalarValue.UNDEFINED, false),
                    new Scalar("5", FASTType.I32, Operator.CONSTANT,
                        new IntegerValue(1), false), /* NON-TRANSFERRABLE */
                new Scalar("6", FASTType.I32, Operator.DEFAULT,
                        new IntegerValue(2), false)
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
                Console.WriteLine("MSG/S:" + (64000 / seconds).ToString("0"));
            }
        }
    }
}
