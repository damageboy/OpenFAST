using System;
using QName = OpenFAST.QName;

namespace OpenFAST.Error
{
	public struct FastConstants
    {
		public readonly static QName ANY_TYPE;
		public readonly static FastAlertSeverity ERROR;
		public readonly static FastAlertSeverity WARN;
		public readonly static FastAlertSeverity FATAL;
		// Error Types
		public readonly static ErrorType DYNAMIC;
		public readonly static ErrorType STATIC;
		public readonly static ErrorType REPORTABLE;
		// Static Errors
		public readonly static ErrorCode S1_INVALID_XML;
		public readonly static ErrorCode S2_OPERATOR_TYPE_INCOMP;
		public readonly static ErrorCode S3_INITIAL_VALUE_INCOMP;
		public readonly static ErrorCode S4_NO_INITIAL_VALUE_FOR_CONST;
		public readonly static ErrorCode S5_NO_INITVAL_MNDTRY_DFALT;
		// Dynamic Errors
		public readonly static ErrorCode D1_FIELD_APP_INCOMP;
		public readonly static ErrorCode D2_INT_OUT_OF_RANGE;
		public readonly static ErrorCode D3_CANT_ENCODE_VALUE;
		public readonly static ErrorCode D4_INVALID_TYPE;
		public readonly static ErrorCode D5_NO_DEFAULT_VALUE;
		public readonly static ErrorCode D6_MNDTRY_FIELD_NOT_PRESENT;
		public readonly static ErrorCode D7_SUBTRCTN_LEN_LONG;
		public readonly static ErrorCode D8_TEMPLATE_NOT_EXIST;
		public readonly static ErrorCode D9_TEMPLATE_NOT_REGISTERED;
		// Reportable Errors
		public readonly static ErrorCode R1_LARGE_DECIMAL;
		public readonly static ErrorCode R4_NUMERIC_VALUE_TOO_LARGE;
		public readonly static ErrorCode R5_DECIMAL_CANT_CONVERT_TO_INT;
		public readonly static ErrorCode R7_PMAP_OVERLONG;
		public readonly static ErrorCode R8_PMAP_TOO_MANY_BITS;
		public readonly static ErrorCode R9_STRING_OVERLONG;
		// Errors not defined in the FAST specification
		public readonly static ErrorCode GENERAL_ERROR;
		public readonly static ErrorCode IMPOSSIBLE_EXCEPTION;
		public readonly static ErrorCode IO_ERROR;
		public readonly static ErrorCode PARSE_ERROR;
		public readonly static string TEMPLATE_DEFINITION_1_1 = "http://www.fixprotocol.org/ns/fast/td/1.1";
		public readonly static QName LENGTH_FIELD;
		static FastConstants()
		{
			ANY_TYPE = new QName("any");
			ERROR = FastAlertSeverity.ERROR;
			WARN = FastAlertSeverity.WARN;
			FATAL = FastAlertSeverity.FATAL;
			DYNAMIC = new ErrorType("Dynamic");
			STATIC = new ErrorType("Static");
			REPORTABLE = new ErrorType("Reportable");
			S1_INVALID_XML = new ErrorCode(OpenFAST.Error.FastConstants.STATIC, 1, "ERR S1", "Invalid XML", OpenFAST.Error.FastConstants.ERROR);
			S2_OPERATOR_TYPE_INCOMP = new ErrorCode(OpenFAST.Error.FastConstants.STATIC, 2, "ERR S2", "Incompatible operator and type", OpenFAST.Error.FastConstants.ERROR);
			S3_INITIAL_VALUE_INCOMP = new ErrorCode(OpenFAST.Error.FastConstants.STATIC, 3, "ERR S3", "Incompatible initial value", OpenFAST.Error.FastConstants.ERROR);
			S4_NO_INITIAL_VALUE_FOR_CONST = new ErrorCode(OpenFAST.Error.FastConstants.STATIC, 4, "ERR S4", "Fields with constant operators must have a default value defined.", OpenFAST.Error.FastConstants.ERROR);
			S5_NO_INITVAL_MNDTRY_DFALT = new ErrorCode(OpenFAST.Error.FastConstants.STATIC, 5, "ERR S5", "No initial value for mandatory field with default operator", OpenFAST.Error.FastConstants.ERROR);
			D1_FIELD_APP_INCOMP = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 1, "ERR D1", "Field cannot be converted to type of application field", OpenFAST.Error.FastConstants.ERROR);
			D2_INT_OUT_OF_RANGE = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 2, "ERR D2", "The integer value is out of range for the specified integer type.", OpenFAST.Error.FastConstants.WARN);
			D3_CANT_ENCODE_VALUE = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 3, "ERR D3", "The value cannot be encoded for the given operator.", OpenFAST.Error.FastConstants.ERROR);
			D4_INVALID_TYPE = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 4, "ERR D4", "The previous value is not the same type as the type of the current field.", OpenFAST.Error.FastConstants.ERROR);
			D5_NO_DEFAULT_VALUE = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 5, "ERR D5", "If no prior value is set and the field is not present, there must be a default value or the optional flag must be set.", OpenFAST.Error.FastConstants.ERROR);
			D6_MNDTRY_FIELD_NOT_PRESENT = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 6, "ERR D6", "A mandatory field must have a value", OpenFAST.Error.FastConstants.ERROR);
			D7_SUBTRCTN_LEN_LONG = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 7, "ERR D7", "The subtraction length is longer than the base value.", OpenFAST.Error.FastConstants.ERROR);
			D8_TEMPLATE_NOT_EXIST = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 8, "ERR D8", "The referenced template does not exist.", OpenFAST.Error.FastConstants.ERROR);
			D9_TEMPLATE_NOT_REGISTERED = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 9, "ERR D9", "The template has not been registered.", OpenFAST.Error.FastConstants.ERROR);
			R1_LARGE_DECIMAL = new ErrorCode(OpenFAST.Error.FastConstants.REPORTABLE, 1, "ERR R1", "Decimal exponent does not fit into range -63...63", OpenFAST.Error.FastConstants.WARN);
			R4_NUMERIC_VALUE_TOO_LARGE = new ErrorCode(OpenFAST.Error.FastConstants.REPORTABLE, 4, "ERR R4", "The value is too large.", OpenFAST.Error.FastConstants.WARN);
			R5_DECIMAL_CANT_CONVERT_TO_INT = new ErrorCode(OpenFAST.Error.FastConstants.REPORTABLE, 5, "ERR R5", "The decimal value cannot convert to an integer because of trailing decimal part.", OpenFAST.Error.FastConstants.WARN);
			R7_PMAP_OVERLONG = new ErrorCode(OpenFAST.Error.FastConstants.REPORTABLE, 7, "ERR R7", "The presence map is overlong.", OpenFAST.Error.FastConstants.WARN);
			R8_PMAP_TOO_MANY_BITS = new ErrorCode(OpenFAST.Error.FastConstants.REPORTABLE, 8, "ERR R8", "The presence map has too many bits.", OpenFAST.Error.FastConstants.WARN);
			R9_STRING_OVERLONG = new ErrorCode(OpenFAST.Error.FastConstants.REPORTABLE, 9, "ERR R9", "The string is overlong.", OpenFAST.Error.FastConstants.ERROR);
			GENERAL_ERROR = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 100, "GENERAL", "An error has occurred.", OpenFAST.Error.FastConstants.ERROR);
			IMPOSSIBLE_EXCEPTION = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 101, "IMPOSSIBLE", "This should never happen.", OpenFAST.Error.FastConstants.ERROR);
			IO_ERROR = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 102, "IOERROR", "An IO error occurred.", OpenFAST.Error.FastConstants.FATAL);
			PARSE_ERROR = new ErrorCode(OpenFAST.Error.FastConstants.DYNAMIC, 103, "PARSEERR", "An exception occurred while parsing.", OpenFAST.Error.FastConstants.ERROR);
			LENGTH_FIELD = new QName("length", OpenFAST.Error.FastConstants.TEMPLATE_DEFINITION_1_1);
		}
	}
}