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
using System.Text;
using OpenFAST.Template.Types;

namespace OpenFAST.Template
{
    public sealed class ComposedScalar : Field
    {
        private const Type ScalarValueType = null;
        private readonly FastType _fastType;
        private readonly Scalar[] _fields;
        private readonly IComposedValueConverter _valueConverter;

        public ComposedScalar(string name, FastType fastType, Scalar[] fields, bool optional,
                              IComposedValueConverter valueConverter)
            : this(new QName(name), fastType, fields, optional, valueConverter)
        {
        }

        public ComposedScalar(QName name, FastType fastType, Scalar[] fields, bool optional,
                              IComposedValueConverter valueConverter) : base(name, optional)
        {
            _fields = fields;
            _valueConverter = valueConverter;
            _fastType = fastType;
        }

        public override string TypeName
        {
            get { return _fastType.Name; }
        }

        public override Type ValueType
        {
            get { return ScalarValueType; }
        }

        public FastType FASTType
        {
            get { return _fastType; }
        }

        public Scalar[] Fields
        {
            get { return _fields; }
        }

        public override bool UsesPresenceMapBit
        {
            get
            {
                for (int i = 0; i < _fields.Length; i++)
                    if (_fields[i].UsesPresenceMapBit)
                        return true;
                return false;
            }
        }

        public override IFieldValue CreateValue(string value)
        {
            return _fastType.GetValue(value);
        }

        public override IFieldValue Decode(Stream inStream, Group decodeTemplate, Context context,
                                           BitVectorReader presenceMapReader)
        {
            var values = new IFieldValue[_fields.Length];
            for (int i = 0; i < _fields.Length; i++)
            {
                values[i] = _fields[i].Decode(inStream, decodeTemplate, context, presenceMapReader);
                if (i == 0 && values[0] == null)
                    return null;
            }
            return _valueConverter.Compose(values);
        }

        public override byte[] Encode(IFieldValue value, Group encodeTemplate, Context context,
                                      BitVectorBuilder presenceMapBuilder)
        {
            if (value == null)
            {
                // Only encode null in the first field.
                return _fields[0].Encode(null, encodeTemplate, context, presenceMapBuilder);
            }
            var buffer = new MemoryStream(_fields.Length*8);
            IFieldValue[] values = _valueConverter.Split(value);
            for (int i = 0; i < _fields.Length; i++)
            {
                try
                {
                    byte[] tempByteArray = _fields[i].Encode(values[i], encodeTemplate, context, presenceMapBuilder);
                    buffer.Write(tempByteArray, 0, tempByteArray.Length);
                }
                catch (IOException e)
                {
                    throw new RuntimeException(e);
                }
            }
            return buffer.ToArray();
        }

        public override bool IsPresenceMapBitSet(byte[] encoding, IFieldValue fieldValue)
        {
            return false;
        }

        public override bool Equals(Object obj)
        {
            if (obj == this)
                return true;
            if (obj == null || !obj.GetType().Equals(typeof (ComposedScalar)))
                return false;
            var other = (ComposedScalar) obj;
            if (other._fields.Length != _fields.Length)
                return false;
            if (!other.Name.Equals(Name))
                return false;
            for (int i = 0; i < _fields.Length; i++)
            {
                Scalar fld1 = _fields[i];
                Scalar fld2 = other._fields[i];

                if (!fld2.FASTType.Equals(fld1.FASTType))
                    return false;
                if (!fld2.TypeCodec.Equals(fld1.TypeCodec))
                    return false;
                if (!fld2.Operator.Equals(fld1.Operator))
                    return false;
                if (!fld2.OperatorCodec.Equals(fld1.OperatorCodec))
                    return false;
                if (!fld2.DefaultValue.Equals(fld1.DefaultValue))
                    return false;
                if (!fld2.Dictionary.Equals(fld1.Dictionary))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return QName.GetHashCode();
        }

        public override string ToString()
        {
            const string separator = ", ";

            var builder = new StringBuilder();
            builder.Append("Composed {");

            foreach (Scalar t in _fields)
            {
                builder.Append(t).Append(separator);
            }
            builder.Remove(builder.Length - separator.Length, builder.Length);

            return builder.Append("}").ToString();
        }
    }
}