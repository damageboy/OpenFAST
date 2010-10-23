using System;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;

namespace OpenFAST.Error
{
    /// <summary>
    /// Generic exception capable of delayed message formatting.
    /// Inherit for more specific exceptions.
    /// </summary>
    [Serializable]
    public class FastException : Exception
    {
        private readonly object[] _arguments;
        private readonly string _formatStr;
        private readonly bool _useFormat;

        [StringFormatMethod("format")]
        private FastException(bool useFormat, Exception inner, string message, params object[] args)
            : base(message, inner)
        {
            _useFormat = useFormat;
            _formatStr = message;
            _arguments = args;
        }

        public FastException()
            : this(false, null, null, null)
        {
        }

        public FastException(string format)
            : this(false, null, format, null)
        {
        }

        [StringFormatMethod("format")]
        public FastException(string format, params object[] args)
            : this(true, null, format, args)
        {
        }

        public FastException(Exception inner, string format)
            : this(false, inner, format, null)
        {
        }

        [StringFormatMethod("format")]
        public FastException(Exception inner, string format, params object[] args)
            : this(true, inner, format, args)
        {
        }

        public override string Message
        {
            get
            {
                if (!_useFormat)
                    return _formatStr;

                try
                {
                    return string.Format(_formatStr, _arguments);
                }
                catch (Exception ex)
                {
                    var sb = new StringBuilder();

                    sb.Append(_formatStr);
                    sb.Append("\nFormatting error: ");
                    sb.Append(ex.Message);
                    if (_arguments != null && _arguments.Length > 0)
                    {
                        sb.Append("\nArguments: ");
                        sb.Append(_arguments[0]);
                        for (int i = 1; i < _arguments.Length; i++)
                            sb.Append(", ").Append(_arguments[i]);
                    }

                    return sb.ToString();
                }
            }
        }

        #region Serialization

        private const string SerializationField = "FormatString";

        protected FastException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _formatStr = (string) info.GetValue(SerializationField, typeof (string));
            // Leave other values at their default
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // To avoid any serialization issues with param objects, format message now
            info.AddValue(SerializationField, Message, typeof (string));
        }

        #endregion
    }
}