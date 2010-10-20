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
using NUnit.Framework;
using OpenFAST.Error;
using OpenFAST.Template.Type;
using UnitTest.Test;

namespace UnitTest.Template
{
    [TestFixture]
    public class TypeTest : OpenFastTestCase
    {
//        [Test]
//        public void TestGetType()
//        {
//            Assert.Equals(FASTType.U32, FASTType.GetType("uInt32"));
//            try
//            {
//                FASTType.GetType("u32");
//                Assert.Fail();
//            }
//            catch (Exception e)
//            {
//                Assert.Equals(
//                    "The type named u32 does not exist.  Existing types are {uInt8,uInt16,uInt32,uInt64,int8,int16,int32,int64,string,ascii,unicode,byteVector,decimal}",
//                    e.Message);
//            }
//        }
//
//        [Test]
//        public void TestIncompatibleDefaultValue()
//        {
//            try
//            {
//                Template("<template>" + "  <decimal><copy value='10a'/></decimal>" + "</template>");
//                Assert.Fail();
//            }
//            catch (FastException e)
//            {
//                Assert.Equals(FastConstants.S3_INITIAL_VALUE_INCOMP, e.Code);
//                Assert.Equals("The value '10a' is not compatible with type decimal", e.Message);
//            }
//        }
    }
}