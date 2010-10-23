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
using OpenFAST.Utility;

namespace OpenFAST.Error
{
    public enum ErrorType
    {
        Static,
        Dynamic,
        Reportable,
        Session,
    }

    public enum Severity
    {
        Fatal = 1,
        Error = 2,
        Warn = 3,
        Info = 4,
    }

    public enum StaticError
    {
        [ErrorInfo(ErrorType.Static, Severity.Error, "ERR S1", "Invalid XML")]
        S1InvalidXml = 1,
        [ErrorInfo(ErrorType.Static, Severity.Error, "ERR S2", "Incompatible operator and type")]
        S2OperatorTypeIncomp = 2,
        [ErrorInfo(ErrorType.Static, Severity.Error, "ERR S3", "Incompatible initial value")]
        S3InitialValueIncomp = 3,
        [ErrorInfo(ErrorType.Static, Severity.Error, "ERR S4", "Fields with constant operators must have a default value defined.")]
        S4NoInitialValueForConst = 4,
        [ErrorInfo(ErrorType.Static, Severity.Error, "ERR S5", "No initial value for mandatory field with default operator")]
        S5NoInitvalMndtryDfalt = 5,

        [ErrorInfo(ErrorType.Static, Severity.Error, "IOERROR", "IO Error")]
        IoError = 10000,
        [ErrorInfo(ErrorType.Static, Severity.Error, "XMLPARSEERR", "XML Parsing Error")]
        XmlParsingError,
        [ErrorInfo(ErrorType.Static, Severity.Error, "INVALIDTYPE", "Invalid Type")]
        InvalidType,
    }

    public enum RepError
    {
        [ErrorInfo(ErrorType.Reportable, Severity.Warn, "ERR R1", "Decimal exponent does not fit into range -63...63")]
        R1LargeDecimal = 1,
        [ErrorInfo(ErrorType.Reportable, Severity.Warn, "ERR R4", "The value is too large.")]
        R4NumericValueTooLarge = 4,
        [ErrorInfo(ErrorType.Reportable, Severity.Warn, "ERR R5", "The decimal value cannot convert to an integer because of trailing decimal part.")]
        R5DecimalCantConvertToInt = 5,
        [ErrorInfo(ErrorType.Reportable, Severity.Warn, "ERR R7", "The presence map is overlong.")]
        R7PmapOverlong = 7,
        [ErrorInfo(ErrorType.Reportable, Severity.Warn, "ERR R8", "The presence map has too many bits.")]
        R8PmapTooManyBits = 8,
        [ErrorInfo(ErrorType.Reportable, Severity.Error, "ERR R9", "The string is overlong.")]
        R9StringOverlong = 9,
    }

    public enum DynError
    {
        [ErrorInfo(ErrorType.Session, Severity.Error, "UNDEFINED", "Undefined Alert Code")]
        Undefined = -1,

        [ErrorInfo(ErrorType.Dynamic, Severity.Error, "ERR D1", "Field cannot be converted to type of application field")]
        D1FieldAppIncomp = 1,
        [ErrorInfo(ErrorType.Dynamic, Severity.Warn, "ERR D2", "The integer value is out of range for the specified integer type.")]
        D2IntOutOfRange = 2,
        [ErrorInfo(ErrorType.Dynamic, Severity.Error, "ERR D3", "The value cannot be encoded for the given operator.")]
        D3CantEncodeValue = 3,
        [ErrorInfo(ErrorType.Dynamic, Severity.Error, "ERR D4", "The previous value is not the same type as the type of the current field.")]
        D4InvalidType = 4,
        [ErrorInfo(ErrorType.Dynamic, Severity.Error, "ERR D5", "If no prior value is set and the field is not present, there must be a default value or the optional flag must be set.")]
        D5NoDefaultValue = 5,
        [ErrorInfo(ErrorType.Dynamic, Severity.Error, "ERR D6", "A mandatory field must have a value")]
        D6MndtryFieldNotPresent = 6,
        [ErrorInfo(ErrorType.Dynamic, Severity.Error, "ERR D7", "The subtraction length is longer than the base value.")]
        D7SubtrctnLenLong = 7,
        [ErrorInfo(ErrorType.Dynamic, Severity.Error, "ERR D8", "The referenced template does not exist.")]
        D8TemplateNotExist = 8,
        [ErrorInfo(ErrorType.Dynamic, Severity.Error, "ERR D9", "The template has not been registered.")]
        D9TemplateNotRegistered = 9,

        // These were defined in SessionConstants, but it appears they should be part of the dynamic enum.
        // TODO: verify?

        [ErrorInfo(ErrorType.Session, Severity.Error, "TNOTSUPP", "Template not supported")]
        TemplateNotSupported = 11,
        [ErrorInfo(ErrorType.Session, Severity.Error, "TUNKNOWN", "Template unknown")]
        TemplateUnknown = 12,
        [ErrorInfo(ErrorType.Session, Severity.Fatal, "EAUTH", "Unauthorized")]
        Unauthorized = 13,
        [ErrorInfo(ErrorType.Session, Severity.Error, "EPROTO", "Protocol Error")]
        ProtcolError = 14,
        [ErrorInfo(ErrorType.Session, Severity.Info, "CLOSE", "Session Closed")]
        Close = 15,

        // Dynamic Errors that are not defined in the FAST specification

        [ErrorInfo(ErrorType.Dynamic, Severity.Error, "GENERAL", "An error has occurred.")]
        GeneralError = 100,
        [ErrorInfo(ErrorType.Dynamic, Severity.Error, "IMPOSSIBLE", "This should never happen.")]
        ImpossibleException = 101,
        [ErrorInfo(ErrorType.Dynamic, Severity.Fatal, "IOERROR", "An IO error occurred.")]
        IoError = 102,
        [ErrorInfo(ErrorType.Dynamic, Severity.Error, "PARSEERR", "An exception occurred while parsing.")]
        ParseError = 103,
    }

    public static class ErrorExt
    {
        public static ErrorInfoAttribute GetErrorAttr(this DynError error)
        {
            return Util.GetEnumSingleAttribute<ErrorInfoAttribute, DynError>(error);
        }

        public static ErrorInfoAttribute GetErrorAttr(this RepError error)
        {
            return Util.GetEnumSingleAttribute<ErrorInfoAttribute, RepError>(error);
        }

        public static ErrorInfoAttribute GetErrorAttr(this StaticError error)
        {
            return Util.GetEnumSingleAttribute<ErrorInfoAttribute, StaticError>(error);
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ErrorInfoAttribute : Attribute
    {
        public ErrorType ErrorType { get; private set; }
        public Severity Severity { get; private set; }
        public string ShortName { get; private set; }
        public string Description { get; private set; }

        public ErrorInfoAttribute(ErrorType errorType, Severity severity, string shortName, string description)
        {
            ErrorType = errorType;
            Severity = severity;
            ShortName = shortName;
            Description = description;
        }
    }
}