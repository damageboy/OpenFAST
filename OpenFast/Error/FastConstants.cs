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

*/
namespace OpenFAST.Error
{
    public struct FastConstants
    {
        // ReSharper disable InconsistentNaming
        public const string TEMPLATE_DEFINITION_1_1 = "http://www.fixprotocol.org/ns/fast/td/1.1";
        public static readonly QName ANY_TYPE = new QName("any");
        public static readonly FastAlertSeverity ERROR = FastAlertSeverity.ERROR;
        public static readonly FastAlertSeverity WARN = FastAlertSeverity.WARN;
        public static readonly FastAlertSeverity FATAL = FastAlertSeverity.FATAL;
        // Error Types
        public static readonly ErrorType DYNAMIC = new ErrorType("Dynamic");
        public static readonly ErrorType STATIC = new ErrorType("Static");
        public static readonly ErrorType REPORTABLE = new ErrorType("Reportable");
        // Static Errors
        public static readonly ErrorCode S1_INVALID_XML;
        public static readonly ErrorCode S2_OPERATOR_TYPE_INCOMP;
        public static readonly ErrorCode S3_INITIAL_VALUE_INCOMP;
        public static readonly ErrorCode S4_NO_INITIAL_VALUE_FOR_CONST;
        public static readonly ErrorCode S5_NO_INITVAL_MNDTRY_DFALT;
        // Dynamic Errors
        public static readonly ErrorCode D1_FIELD_APP_INCOMP;
        public static readonly ErrorCode D2_INT_OUT_OF_RANGE;
        public static readonly ErrorCode D3_CANT_ENCODE_VALUE;
        public static readonly ErrorCode D4_INVALID_TYPE;
        public static readonly ErrorCode D5_NO_DEFAULT_VALUE;
        public static readonly ErrorCode D6_MNDTRY_FIELD_NOT_PRESENT;
        public static readonly ErrorCode D7_SUBTRCTN_LEN_LONG;
        public static readonly ErrorCode D8_TEMPLATE_NOT_EXIST;
        public static readonly ErrorCode D9_TEMPLATE_NOT_REGISTERED;
        // Reportable Errors
        public static readonly ErrorCode R1_LARGE_DECIMAL;
        public static readonly ErrorCode R4_NUMERIC_VALUE_TOO_LARGE;
        public static readonly ErrorCode R5_DECIMAL_CANT_CONVERT_TO_INT;
        public static readonly ErrorCode R7_PMAP_OVERLONG;
        public static readonly ErrorCode R8_PMAP_TOO_MANY_BITS;
        public static readonly ErrorCode R9_STRING_OVERLONG;
        // Errors not defined in the FAST specification
        public static readonly ErrorCode GENERAL_ERROR;
        public static readonly ErrorCode IMPOSSIBLE_EXCEPTION;
        public static readonly ErrorCode IO_ERROR;
        public static readonly ErrorCode PARSE_ERROR;
        public static readonly QName LENGTH_FIELD;
        // ReSharper restore InconsistentNaming

        static FastConstants()
        {
            S1_INVALID_XML = new ErrorCode(STATIC, 1, "ERR S1", "Invalid XML", ERROR);
            S2_OPERATOR_TYPE_INCOMP = new ErrorCode(STATIC, 2, "ERR S2", "Incompatible operator and type", ERROR);
            S3_INITIAL_VALUE_INCOMP = new ErrorCode(STATIC, 3, "ERR S3", "Incompatible initial value", ERROR);
            S4_NO_INITIAL_VALUE_FOR_CONST = new ErrorCode(
                STATIC, 4, "ERR S4", "Fields with constant operators must have a default value defined.",
                ERROR);
            S5_NO_INITVAL_MNDTRY_DFALT = new ErrorCode(
                STATIC, 5, "ERR S5", "No initial value for mandatory field with default operator",
                ERROR);
            D1_FIELD_APP_INCOMP = new ErrorCode(
                DYNAMIC, 1, "ERR D1", "Field cannot be converted to type of application field", ERROR);
            D2_INT_OUT_OF_RANGE = new ErrorCode(
                DYNAMIC, 2, "ERR D2", "The integer value is out of range for the specified integer type.",
                WARN);
            D3_CANT_ENCODE_VALUE = new ErrorCode(
                DYNAMIC, 3, "ERR D3", "The value cannot be encoded for the given operator.", ERROR);
            D4_INVALID_TYPE = new ErrorCode(
                DYNAMIC, 4, "ERR D4", "The previous value is not the same type as the type of the current field.", ERROR);
            D5_NO_DEFAULT_VALUE = new ErrorCode(
                DYNAMIC, 5, "ERR D5",
                "If no prior value is set and the field is not present, there must be a default value or the optional flag must be set.",
                ERROR);
            D6_MNDTRY_FIELD_NOT_PRESENT = new ErrorCode(
                DYNAMIC, 6, "ERR D6", "A mandatory field must have a value", ERROR);
            D7_SUBTRCTN_LEN_LONG = new ErrorCode(
                DYNAMIC, 7, "ERR D7", "The subtraction length is longer than the base value.", ERROR);
            D8_TEMPLATE_NOT_EXIST = new ErrorCode(
                DYNAMIC, 8, "ERR D8", "The referenced template does not exist.", ERROR);
            D9_TEMPLATE_NOT_REGISTERED = new ErrorCode(
                DYNAMIC, 9, "ERR D9", "The template has not been registered.", ERROR);
            R1_LARGE_DECIMAL = new ErrorCode(
                REPORTABLE, 1, "ERR R1",
                "Decimal exponent does not fit into range -63...63", WARN);
            R4_NUMERIC_VALUE_TOO_LARGE = new ErrorCode(
                REPORTABLE, 4, "ERR R4", "The value is too large.", WARN);
            R5_DECIMAL_CANT_CONVERT_TO_INT = new ErrorCode(
                REPORTABLE, 5, "ERR R5",
                "The decimal value cannot convert to an integer because of trailing decimal part.", WARN);
            R7_PMAP_OVERLONG = new ErrorCode(REPORTABLE, 7, "ERR R7", "The presence map is overlong.", WARN);
            R8_PMAP_TOO_MANY_BITS = new ErrorCode(REPORTABLE, 8, "ERR R8", "The presence map has too many bits.", WARN);
            R9_STRING_OVERLONG = new ErrorCode(REPORTABLE, 9, "ERR R9", "The string is overlong.", ERROR);
            GENERAL_ERROR = new ErrorCode(DYNAMIC, 100, "GENERAL", "An error has occurred.", ERROR);
            IMPOSSIBLE_EXCEPTION = new ErrorCode(DYNAMIC, 101, "IMPOSSIBLE", "This should never happen.", ERROR);
            IO_ERROR = new ErrorCode(DYNAMIC, 102, "IOERROR", "An IO error occurred.", FATAL);
            PARSE_ERROR = new ErrorCode(DYNAMIC, 103, "PARSEERR", "An exception occurred while parsing.", ERROR);
            LENGTH_FIELD = new QName("length", TEMPLATE_DEFINITION_1_1);
        }
    }
}