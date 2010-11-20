/*

The contents of this file are subject to the Mozilla Public License
Version 1.1 (the "License"); you may not use this file except in
compliance with the License. You may obtain a copy of the License at
http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS"
basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
License for the specific language governing rights and limitations
under the License.

The Original Code is OpenFAST.

The Initial Developer of the Original Code is The LaSalle Technology
Group, LLC.  Portions created by Shariq Muhammad
are Copyright (C) Shariq Muhammad. All Rights Reserved.

Contributor(s): Shariq Muhammad <shariq.muhammad@gmail.com>
                Yuri Astrakhan <FirstName><LastName>@gmail.com
*/
using System;
using OpenFAST.Template;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;

namespace OpenFAST.UnitTests.Test
{
    public static class ObjectMother
    {
        private static MessageTemplate _quoteTemplate;
        private static MessageTemplate _allocationInstruction;
        private static Group _instrument;
        private static Sequence _allocations;
        private static MessageTemplate _batchTemplate;
        private static MessageTemplate _headerTemplate;
        public static readonly int QuoteTemplateId = 10;
        public const int AllocInstrctnTemplateId = 25;

        public static MessageTemplate QuoteTemplate
        {
            get
            {
                return
                    _quoteTemplate ??
                    (_quoteTemplate =
                     new MessageTemplate(
                         "Quote",
                         new Field[]
                             {
                                 new Scalar("bid", FastType.Decimal, Operator.Delta, ScalarValue.Undefined, false),
                                 new Scalar("ask", FastType.Decimal, Operator.Delta, ScalarValue.Undefined, false)
                             }));
            }
        }

        public static MessageTemplate BatchTemplate
        {
            get
            {
                return
                    _batchTemplate ??
                    (_batchTemplate =
                     new MessageTemplate(
                         "Batch",
                         new Field[]
                             {
                                 new StaticTemplateReference(HeaderTemplate),
                                 new Sequence("Batch", new Field[] {DynamicTemplateReference.Instance}, false)
                             }));
            }
        }

        public static MessageTemplate HeaderTemplate
        {
            get
            {
                return
                    _headerTemplate ??
                    (_headerTemplate =
                     new MessageTemplate(
                         "Header",
                         new Field[]
                             {
                                 new Scalar("Sent", FastType.U32, Operator.Delta, ScalarValue.Undefined, false)
                             }));
            }
        }

        public static MessageTemplate AllocationInstruction
        {
            get
            {
                return
                    _allocationInstruction ??
                    (_allocationInstruction =
                     new MessageTemplate(
                         "AllocInstrctn",
                         new Field[]
                             {
                                 Allocations,
                                 Instrument,
                                 new Scalar("ID", FastType.Ascii, Operator.Delta, ScalarValue.Undefined, false),
                                 new Scalar("Side", FastType.U32, Operator.Copy, ScalarValue.Undefined, false),
                                 new Scalar("Quantity", FastType.Decimal, Operator.Delta, ScalarValue.Undefined, false),
                                 new Scalar("Average Price", FastType.Decimal, Operator.Delta, ScalarValue.Undefined,
                                            false)
                             }));
            }
        }

        public static Sequence Allocations
        {
            get
            {
                return
                    _allocations ??
                    (_allocations =
                     new Sequence(
                         "Allocations",
                         new Field[]
                             {
                                 new Scalar("Account", FastType.Ascii, Operator.Copy, ScalarValue.Undefined, false),
                                 new Scalar("Price", FastType.Decimal, Operator.Delta, ScalarValue.Undefined, false),
                                 new Scalar("Quantity", FastType.Decimal, Operator.Delta, ScalarValue.Undefined, false),
                                 new Scalar("Average Price", FastType.Decimal, Operator.Delta, ScalarValue.Undefined,
                                            false)
                             }, false));
            }
        }

        private static Group Instrument
        {
            get
            {
                return
                    _instrument ??
                    (_instrument =
                     new Group(
                         "Instrmt",
                         new Field[]
                             {
                                 new Scalar("Symbol", FastType.Ascii, Operator.Copy, ScalarValue.Undefined, false),
                                 new Scalar("MMY", FastType.Ascii, Operator.Delta, ScalarValue.Undefined, false),
                             }, false));
            }
        }

        public static Message Quote(double bid, double ask)
        {
            var quote = new Message(QuoteTemplate);
            quote.SetDecimal(1, bid);
            quote.SetDecimal(2, ask);

            return quote;
        }

        public static Message NewAllocInstrctn(String id, int side,
                                               double quantity, double averagePrice, GroupValue instrument,
                                               SequenceValue allocations)
        {
            var allocInstrctn = new Message(AllocationInstruction);
            allocInstrctn.SetFieldValue(1, allocations);
            allocInstrctn.SetFieldValue(2, instrument);
            allocInstrctn.SetFieldValue(3, new StringValue(id));
            allocInstrctn.SetFieldValue(4, new IntegerValue(side));
            allocInstrctn.SetFieldValue(5, new DecimalValue(quantity));
            allocInstrctn.SetFieldValue(6, new DecimalValue(averagePrice));

            return allocInstrctn;
        }

        public static GroupValue NewInstrument(String symbol, String mmy)
        {
            return new GroupValue(Instrument,
                                  new IFieldValue[] {new StringValue(symbol), new StringValue(mmy)});
        }

        public static GroupValue NewAllocation(String account, double price,
                                               double quantity)
        {
            StringValue acct = account != null ? new StringValue(account) : null;
            return new GroupValue(
                Allocations.Group,
                new IFieldValue[]
                    {
                        acct, new DecimalValue(price), new DecimalValue(quantity), new DecimalValue(0.0)
                    });
        }

        public static Message BasicAllocationInstruction()
        {
            return NewAllocInstrctn("abcd1234", 2, 25.0, 102.0, BasicInstrument(), BasicAllocations());
        }

        private static SequenceValue BasicAllocations()
        {
            var value = new SequenceValue(AllocationInstruction.GetSequence("Allocations"));
            value.Add(NewAllocation("general", 101.0, 15.0));
            value.Add(NewAllocation("specific", 103.0, 10.0));
            return value;
        }

        private static GroupValue BasicInstrument()
        {
            return NewInstrument("IBM", "200301");
        }
    }
}