namespace Webkasi.Refactoring.Rules;

[TestClass]
public sealed class CPPRuleTest : RuleTest
{
    #region RemoveMethodClassName
    [TestMethod]
    [DataRow("void CDsClass::DoSomething()", "void DoSomething()")]
    [DataRow("void CDsClass::DoSomething(int x, CDsClass& refClass)", "void DoSomething(int x, CDsClass& refClass)")]
    [DataRow("void CDsClass::DoSomething(int x,\r\n", "void DoSomething(int x,\r\n")]
    [DataRow("\t void CDsClass::DoSomething(int x,\r\n", "\t void DoSomething(int x,\r\n")]
    public void RemoveMethodClassName(string toChange, string expected)
    {
        string result = Apply("RemoveMethodClassName", toChange);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("void CDsClass::DoSomething()", "void DoSomething()")]
    [DataRow("void CDsClass::DoSomething(BOOL x = FALSE, CDsClass& refClass)", "void DoSomething(bool x = false, CDsClass refClass)")]
    [DataRow("void CDsClass::DoSomething(DWORD x,\r\n", "void DoSomething(uint x,\r\n")]
    [DataRow("\t void CDsClass::DoSomething(INT x,\r\n", "\t void DoSomething(int x,\r\n")]
    public void RemoveMethodClassNameApplyAll(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ReplaceStatic_cast
    [TestMethod]
    [DataRow("\tx = static_cast<LPCTSTR>(s);\r\n", "\tx = (string)(s);\r\n")]
    [DataRow("\tx = Test(static_cast<LPCTSTR>(s) + abc);\r\n", "\tx = Test((string)(s) + abc);\r\n")]
    public void ReplaceStatic_cast(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ReplaceConst_cast
    [TestMethod]
    [DataRow("\tint nKeyIndex = const_cast<CDsDataInfo*>(this)->GetIndex(strKey);\r\n", "\tint nKeyIndex = (this).GetIndex(strKey);\r\n")]
    [DataRow("\tint nKeyIndex = const_cast<CDsDataInfo?>(this).GetIndex(strKey);\r\n", "\tint nKeyIndex = (this).GetIndex(strKey);\r\n")]
    public void ReplaceConst_cast(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ReplaceConstMethod
    [TestMethod]
    [DataRow("\tCString Method(INT x, double y) const;\r\n", "\tCString Method(int x, double y);\r\n")]
    [DataRow("\tCString Method(INT x, double y)  const  ;\r\n", "\tCString Method(int x, double y)   ;\r\n")]
    public void ReplaceConstMethod(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ReplaceSimpleClassInstance
    [TestMethod]
    [DataRow("\tCMyClass myClass;", "\tCMyClass myClass = new CMyClass();")]
    [DataRow("\tMyClass myClass;", "\tMyClass myClass;")]
    [DataRow("\tCMyClass myClass; // Test", "\tCMyClass myClass = new CMyClass(); // Test")]
    public void ReplaceSimpleClassInstance(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangeScalarTypeRef
    [TestMethod]
    [DataRow("void Test(int& x) {", "void Test(ref int x) {")]
    [DataRow("void Test(int& x, double& y) {", "void Test(ref int x, ref double y) {")]
    [DataRow("void Test(int & x, double & y) {", "void Test(ref int x, ref double y) {")]
    public void ChangeScalarTypeRef(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangeDelete
    [TestMethod]
    [DataRow("delete[] ptr;", "ptr = null;")]
    [DataRow("delete ptr;", "ptr = null;")]
    [DataRow("delete ptr; // Test", "ptr = null; // Test")]
    [DataRow("\tdelete ptr; // Test", "\tptr = null; // Test")]
    [DataRow("\tx = 10;\r\n\tdelete ptr; // Test", "\tx = 10;\r\n\tptr = null; // Test")]
    public void ChangeDelete(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangePointerDecl
    [TestMethod]
    [DataRow("int* ptr;", "int? ptr;")]
    [DataRow("CString* ptr;", "CString? ptr;")]
    [DataRow("void Test(CString* ptr) // test", "void Test(CString? ptr) // test")]
    [DataRow("\ta = 10;\r\nvoid Test(CString* ptr) // test", "\ta = 10;\r\nvoid Test(CString? ptr) // test")]
    public void ChangePointerDecl(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangePointerAccess
    [TestMethod]
    [DataRow("ptr->CallTest();", "ptr.CallTest();")]
    [DataRow("x = ptr1->ptr2->m_str;", "x = ptr1.ptr2.m_str;")]
    public void ChangePointerAccess(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ChangeArrayDecl
    [TestMethod]
    [DataRow("int xy[12];", "int[] xy = new int[12];")]
    [DataRow("\tint xy [12]; // test", "\tint[] xy = new int[12]; // test")]
    [DataRow("\ta = 10;\r\n\tint xy [12]; // test", "\ta = 10;\r\n\tint[] xy = new int[12]; // test")]
    [DataRow("\ta = 10;\r\n\tint xy [12]; // test\r\n\ta = 10;\r\n", "\ta = 10;\r\n\tint[] xy = new int[12]; // test\r\n\ta = 10;\r\n")]
    public void ChangeArrayDecl(string toChange, string expected)
    {
        string result = Apply(toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion

    #region ConvertVisibility
    [TestMethod]
    [DataRow(
        "protected:\r\nvoid Test1();\r\nvoid Test2();", 
        "protected void Test1()\r\n{ throw new NotImplementedException(); }\r\nprotected void Test2()\r\n{ throw new NotImplementedException(); }")]
    [DataRow(
        "protected:\r\nvoid Test1(\r\n\tint x\r\n);\r\nvoid Test2(\r\n\tint x\r\n\tint y\r\n);",
        "protected void Test1(\r\n\tint x\r\n)\r\n{ throw new NotImplementedException(); }\r\nprotected void Test2(\r\n\tint x\r\n\tint y\r\n)\r\n{ throw new NotImplementedException(); }")]
    [DataRow(
        "protected:\r\nvoid Test1();\r\npublic:\r\nvoid Test2();\r\n", 
        "protected void Test1()\r\n{ throw new NotImplementedException(); }\r\npublic void Test2()\r\n{ throw new NotImplementedException(); }")]
    [DataRow(
        "void Test1();\r\npublic:\r\nvoid Test2();\r\n",
        "private void Test1()\r\n{ throw new NotImplementedException(); }\r\npublic void Test2()\r\n{ throw new NotImplementedException(); }")]
    public void ConvertVisibility(string toChange, string expected)
    {
        // Kein AutoApply!
        string result = Apply("ConvertVisibility", toChange);
        Assert.AreEqual(expected, result);
    }
    #endregion
}
