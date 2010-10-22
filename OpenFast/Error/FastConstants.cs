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
namespace OpenFAST.Error
{
    public struct FastConstants
    {
        public const string TemplateDefinition11 = "http://www.fixprotocol.org/ns/fast/td/1.1";

        public static readonly QName AnyType = new QName("any");
        public static readonly QName LengthField = new QName("length", TemplateDefinition11);

        // Static Errors
        public static readonly ErrorCode S1InvalidXml;
        public static readonly ErrorCode S2OperatorTypeIncomp;
        public static readonly ErrorCode S3InitialValueIncomp;
        public static readonly ErrorCode S4NoInitialValueForConst;
        public static readonly ErrorCode S5NoInitvalMndtryDfalt;
        
        // Dynamic Errors
        public static readonly ErrorCode D1FieldAppIncomp;
        public static readonly ErrorCode D2IntOutOfRange;
        public static readonly ErrorCode D3CantEncodeValue;
        public static readonly ErrorCode D4InvalidType;
        public static readonly ErrorCode D5NoDefaultValue;
        public static readonly ErrorCode D6MndtryFieldNotPresent;
        public static readonly ErrorCode D7SubtrctnLenLong;
        public static readonly ErrorCode D8TemplateNotExist;
        public static readonly ErrorCode D9TemplateNotRegistered;
        
        // Reportable Errors
        public static readonly ErrorCode R1LargeDecimal;
        public static readonly ErrorCode R4NumericValueTooLarge;
        public static readonly ErrorCode R5DecimalCantConvertToInt;
        public static readonly ErrorCode R7PmapOverlong;
        public static readonly ErrorCode R8PmapTooManyBits;
        public static readonly ErrorCode R9StringOverlong;

        // Errors not defined in the FAST specification
        public static readonly ErrorCode GeneralError;
        public static readonly ErrorCode ImpossibleException;
        public static readonly ErrorCode IoError;
        public static readonly ErrorCode ParseError;

        static FastConstants()
        {
            S1InvalidXml = new ErrorCode(
                ErrorType.Static, 1, Severity.Error, "ERR S1", "Invalid XML");
            S2OperatorTypeIncomp = new ErrorCode(
                ErrorType.Static, 2, Severity.Error, "ERR S2", "Incompatible operator and type");
            S3InitialValueIncomp = new ErrorCode(
                ErrorType.Static, 3, Severity.Error, "ERR S3", "Incompatible initial value");
            S4NoInitialValueForConst = new ErrorCode(
                ErrorType.Static, 4, Severity.Error, "ERR S4",
                "Fields with constant operators must have a default value defined.");
            S5NoInitvalMndtryDfalt = new ErrorCode(
                ErrorType.Static, 5, Severity.Error, "ERR S5",
                "No initial value for mandatory field with default operator");
            D1FieldAppIncomp = new ErrorCode(
                ErrorType.Dynamic, 1, Severity.Error, "ERR D1", "Field cannot be converted to type of application field");
            D2IntOutOfRange = new ErrorCode(
                ErrorType.Dynamic, 2, Severity.Warn, "ERR D2",
                "The integer value is out of range for the specified integer type.");
            D3CantEncodeValue = new ErrorCode(
                ErrorType.Dynamic, 3, Severity.Error, "ERR D3", "The value cannot be encoded for the given operator.");
            D4InvalidType = new ErrorCode(
                ErrorType.Dynamic, 4, Severity.Error, "ERR D4",
                "The previous value is not the same type as the type of the current field.");
            D5NoDefaultValue = new ErrorCode(
                ErrorType.Dynamic, 5, Severity.Error, "ERR D5",
                "If no prior value is set and the field is not present, there must be a default value or the optional flag must be set.");
            D6MndtryFieldNotPresent = new ErrorCode(
                ErrorType.Dynamic, 6, Severity.Error, "ERR D6", "A mandatory field must have a value");
            D7SubtrctnLenLong = new ErrorCode(
                ErrorType.Dynamic, 7, Severity.Error, "ERR D7", "The subtraction length is longer than the base value.");
            D8TemplateNotExist = new ErrorCode(
                ErrorType.Dynamic, 8, Severity.Error, "ERR D8", "The referenced template does not exist.");
            D9TemplateNotRegistered = new ErrorCode(
                ErrorType.Dynamic, 9, Severity.Error, "ERR D9", "The template has not been registered.");
            R1LargeDecimal = new ErrorCode(
                ErrorType.Reportable, 1, Severity.Warn, "ERR R1", "Decimal exponent does not fit into range -63...63");
            R4NumericValueTooLarge = new ErrorCode(
                ErrorType.Reportable, 4, Severity.Warn, "ERR R4", "The value is too large.");
            R5DecimalCantConvertToInt = new ErrorCode(
                ErrorType.Reportable, 5, Severity.Warn, "ERR R5",
                "The decimal value cannot convert to an integer because of trailing decimal part.");
            R7PmapOverlong = new ErrorCode(
                ErrorType.Reportable, 7, Severity.Warn, "ERR R7", "The presence map is overlong.");
            R8PmapTooManyBits = new ErrorCode(
                ErrorType.Reportable, 8, Severity.Warn, "ERR R8", "The presence map has too many bits.");
            R9StringOverlong = new ErrorCode(
                ErrorType.Reportable, 9, Severity.Error, "ERR R9", "The string is overlong.");
            GeneralError = new ErrorCode(
                ErrorType.Dynamic, 100, Severity.Error, "GENERAL", "An error has occurred.");
            ImpossibleException = new ErrorCode(
                ErrorType.Dynamic, 101, Severity.Error, "IMPOSSIBLE", "This should never happen.");
            IoError = new ErrorCode(
                ErrorType.Dynamic, 102, Severity.Fatal, "IOERROR", "An IO error occurred.");
            ParseError = new ErrorCode(
                ErrorType.Dynamic, 103, Severity.Error, "PARSEERR", "An exception occurred while parsing.");
        }
    }
}