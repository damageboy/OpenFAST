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
using System.IO;
using OpenFAST.Error;
using OpenFAST.Template.Operators;
using OpenFAST.Template.Types;

namespace OpenFAST.Template
{
    public sealed class MessageTemplate : Group, IFieldSet, IEquatable<MessageTemplate>
    {
        public MessageTemplate(QName name, string childNamespace, Field[] fields)
            : base(name, childNamespace, CloneAndAddTemplateIdField(fields), false)
        {
            foreach (Field f in FieldDefinitions)
                f.AttachToTemplate(this);
        }

        public MessageTemplate(QName name, Field[] fields)
            : this(name, "", fields)
        {
        }

        public MessageTemplate(string name, Field[] fields)
            : this(new QName(name), "", fields)
        {
        }

        public MessageTemplate(string name, string childNamespace, Field[] fields)
            : this(new QName(name), childNamespace, fields)
        {
        }

        #region Cloning

        public MessageTemplate(MessageTemplate other)
            : base(other)
        {
        }

        public override Field Clone()
        {
            return new MessageTemplate(this);
        }

        #endregion

        public new static Type ValueType
        {
            get { return typeof (Message); }
        }

        public Field[] TemplateFields
        {
            get
            {
                var f = new Field[Fields.Length - 1];
                Array.Copy(Fields, 1, f, 0, Fields.Length - 1);
                return f;
            }
        }

        protected override bool UsesPresenceMap
        {
            get { return true; }
        }

        #region IFieldSet Members

        public Field GetField(int index)
        {
            return Fields[index];
        }

        #endregion

        /// <summary> Clone fields and prepend templateId field </summary>
        private static Field[] CloneAndAddTemplateIdField(Field[] fields)
        {
            var newFields = new Field[fields.Length + 1];
            newFields[0] = new Scalar("templateId", FastType.U32, Operator.Copy, ScalarValue.Undefined, false);
            for (int i = 0; i < fields.Length; i++)
                newFields[i + 1] = fields[i].Clone();
            return newFields;
        }

        public byte[] Encode(Message message, Context context)
        {
            int id;
            if (context.TemplateRegistry.TryGetId(message.Template, out id))
            {
                message.SetInteger(0, id);
                return Encode(message, this, context);
            }

            throw new DynErrorException(DynError.TemplateNotRegistered,
                                        "Cannot encode message: The template {0} has not been registered.",
                                        message.Template);
        }

        public Message Decode(Stream inStream, int templateId, BitVectorReader presenceMapReader, Context context)
        {
            try
            {
                if (context.TraceEnabled)
                    context.DecodeTrace.GroupStart(this);
                IFieldValue[] fieldValues = DecodeFieldValues(inStream, this, presenceMapReader, context);
                fieldValues[0] = new IntegerValue(templateId);
                var message = new Message(this, fieldValues);
                if (context.TraceEnabled)
                    context.DecodeTrace.GroupEnd();
                return message;
            }
            catch (DynErrorException e)
            {
                throw new DynErrorException(e, e.Error, "An error occurred while decoding {0}", this);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public override IFieldValue CreateValue(string value)
        {
            return new Message(this);
        }

        #region Equals

        public bool Equals(MessageTemplate other)
        {
            if (!Name.Equals(other.Name))
                return false;
            if (Fields.Length != other.Fields.Length)
                return false;
            for (int i = 0; i < Fields.Length; i++)
            {
                if (!Fields[i].Equals(other.Fields[i]))
                    return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as MessageTemplate);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}