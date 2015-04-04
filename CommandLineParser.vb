'
'  KVList.vb - Key-Value List
'
'  Project: 
'  Author: S.Zabinskis
'
Imports System.Collections.Generic


Public MustInherit Class TBaseKeyValue

    Enum TValueTypes
        vtVoid
        vtString
        vtInt
    End Enum

#Region "Local Variables"
    Protected _valueType As TValueTypes
    Protected _required As Boolean = False
    Protected _initialized As Boolean = False
    Protected _unique As Boolean = False
#End Region


#Region "Ctors"
    Public Sub New(ByVal valueType As TValueTypes, ByVal amust As Boolean, Optional ByVal unique As Boolean = False)
        _valueType = valueType
        _required = amust
        _unique = unique
    End Sub

    Private Sub New()
    End Sub
#End Region

    Public MustOverride Function Count() As Integer


#Region "Properties"

    Public ReadOnly Property ValueType() As TValueTypes
        Get
            Return _valueType
        End Get
    End Property

    Public ReadOnly Property Initialized() As Boolean
        Get
            Return _initialized
        End Get
    End Property


    Public ReadOnly Property Required() As Boolean
        Get
            Return _required
        End Get
    End Property

    Public ReadOnly Property Unique() As Boolean
        Get
            Return _unique
        End Get
    End Property
#End Region

    Public Overridable Sub Clear()
        _initialized = False
    End Sub

End Class

Public Class TVoidKeyValue : Inherits TBaseKeyValue

#Region "Ctors"
    Public Sub New()
        MyBase.New(TValueTypes.vtVoid, False, True)
    End Sub
#End Region

#Region "Methods"
    Public Function AddValue() As Boolean
        If _initialized Then
            Return False
        End If
        _initialized = True
        Return True
    End Function

    Public Overrides Function Count() As Integer
        Return 0
    End Function

#End Region

End Class



Public Class TClassKeyValue(Of T) : Inherits TBaseKeyValue

#Region "Local Variables"
    Private _values As New List(Of T)
#End Region

#Region "Ctors"
    Public Sub New(ByVal valueType As TValueTypes, ByVal amust As Boolean, Optional ByVal unique As Boolean = True)
        MyBase.New(valueType, amust, unique)
    End Sub
#End Region

#Region "Methods"

    ' trivial validation function
    Protected Overridable Function Validate(ByVal value As T) As Boolean
        Return True
    End Function

    Public Function AddValue(ByVal value As T) As Boolean
        If _unique And _initialized Then
            Return False
        End If
        If Validate(value) Then
            _values.Add(value)
            _initialized = True
            Return True
        End If
        Return False
    End Function


    Public Function GetValue(Optional ByVal index As Integer = 0) As T
        Return _values(index)
    End Function

    Public Overrides Function Count() As Integer
        Return _values.Count
    End Function

    Public Overrides Sub Clear()
        MyBase.Clear()
        _values.Clear()
    End Sub
#End Region

End Class

Public Class TPosIntKeyValue : Inherits TClassKeyValue(Of Integer)

#Region "Ctors"
    Public Sub New(ByVal amust As Boolean, Optional ByVal unique As Boolean = True)
        MyBase.New(TValueTypes.vtInt, amust, unique)
    End Sub
#End Region

#Region "Methods"

    Protected Overrides Function Validate(ByVal value As Integer) As Boolean
        Return value > 0
    End Function

#End Region

End Class

Public Class TBoundedIntKeyValue : Inherits TClassKeyValue(Of Integer)

#Region "Local Variables"
    Private _lwr, _upr As Integer
#End Region

#Region "Ctors"
    Public Sub New(ByVal lwr As Integer, ByVal upr As Integer, ByVal amust As Boolean, Optional ByVal unique As Boolean = True)
        MyBase.New(TValueTypes.vtInt, amust, unique)
        _lwr = lwr
        _upr = upr
    End Sub
#End Region


#Region "Methods"

    Protected Overrides Function Validate(ByVal value As Integer) As Boolean
        Return value >= _lwr AndAlso value <= _upr
    End Function

#End Region

End Class

Public Class TKeyValueList

#Region "Local Variables"
    Protected Shared _valid As String = "_qwertyuiopasdfghjklzxcvbnm"
    Protected _kvlist As New Dictionary(Of String, TBaseKeyValue)
    Protected _testId As Boolean = False ' perform or not parameter name validation 
#End Region

#Region "Ctors"
    Public Sub New(Optional ByVal testId As Boolean = False)
        _testId = testId
    End Sub
#End Region

    Protected Function ValidateId(ByVal name As String) As Boolean
        If _testId Then
            If name <> String.Empty AndAlso _valid.IndexOf(name.ToLower.Substring(0, 1)) Then
                If name.StartsWith("_") Then
                    Dim tempName As String = name.TrimStart("_").ToLower()
                    If tempName.Length() > 0 Then
                        If Not tempName.StartsWith("qwertyuiopasdfghjklzxcvbnm") Then
                            Return False
                        End If
                    Else
                        Return False
                    End If
                End If
                Return True
            End If
            Return False
        End If
        Return True
    End Function


#Region "Add Parameters (commandline options)"

    Protected Sub BeforeAddParam(ByVal name As String)
        If ValidateId(name) Then
            If _kvlist.ContainsKey(name.ToLower()) Then
                Throw New Exception(String.Format("Parameter {0} already exists!", name))
            End If
        Else
            Throw New Exception(String.Format("Ill formed parameter name: {0}", name))
        End If
    End Sub

    Protected Sub AddClassParam(Of T)(ByVal valueType As TBaseKeyValue.TValueTypes, ByVal name As String, ByVal amust As Boolean, ByVal uniq As Boolean)
        BeforeAddParam(name)
        _kvlist.Add(name.ToLower(), New TClassKeyValue(Of T)(valueType, amust, uniq))
    End Sub


    Public Sub AddVoid(ByVal name As String)
        BeforeAddParam(name)
        _kvlist.Add(name.ToLower, New TVoidKeyValue())
    End Sub

    Public Sub AddInt(ByVal name As String, Optional ByVal amust As Boolean = False)
        AddClassParam(Of Integer)(TBaseKeyValue.TValueTypes.vtInt, name, amust, True)
    End Sub

    Public Sub AddIntRep(ByVal name As String, Optional ByVal amust As Boolean = False)
        AddClassParam(Of Integer)(TBaseKeyValue.TValueTypes.vtInt, name, amust, False)
    End Sub

    Protected Sub AddPosIntParam(ByVal name As String, ByVal amust As Boolean, ByVal uniq As Boolean)
        BeforeAddParam(name)
        _kvlist.Add(name.ToLower(), New TPosIntKeyValue(amust, uniq))
    End Sub

    Public Sub AddPosInt(ByVal name As String, Optional ByVal amust As Boolean = False)
        AddPosIntParam(name, amust, True)
    End Sub

    Public Sub AddPosIntRep(ByVal name As String, Optional ByVal amust As Boolean = False)
        AddPosIntParam(name, amust, False)
    End Sub


    Protected Sub AddBoundedIntParam(ByVal name As String, ByVal lwr As Integer, ByVal upr As Integer, ByVal amust As Boolean, ByVal uniq As Boolean)
        BeforeAddParam(name)
        _kvlist.Add(name.ToLower(), New TBoundedIntKeyValue(lwr, upr, amust, uniq))
    End Sub

    Public Sub AddBoundedInt(ByVal name As String, ByVal lwr As Integer, ByVal upr As Integer, Optional ByVal amust As Boolean = False)
        AddBoundedIntParam(name, lwr, upr, amust, True)
    End Sub

    Public Sub AddBoundedIntRep(ByVal name As String, ByVal lwr As Integer, ByVal upr As Integer, Optional ByVal amust As Boolean = False)
        AddBoundedIntParam(name, lwr, upr, amust, False)
    End Sub

    Public Sub AddString(ByVal name As String, Optional ByVal amust As Boolean = False)
        AddClassParam(Of String)(TBaseKeyValue.TValueTypes.vtString, name, amust, True)
    End Sub

    Public Sub AddStringRep(ByVal name As String, Optional ByVal amust As Boolean = False)
        AddClassParam(Of String)(TBaseKeyValue.TValueTypes.vtString, name, amust, False)
    End Sub

#End Region

#Region "Clear Parameters"

    Public Overloads Sub Clear()
        _kvlist.Clear()
    End Sub


    Public Overloads Sub Clear(ByVal name As String)
        _kvlist.Remove(name.ToLower)
    End Sub

#End Region


#Region "Add Values"

    Protected Sub AddClassValue(Of T)(ByVal name As String, ByVal value As T)
        If _kvlist.ContainsKey(name.ToLower) Then
            If Not CType(_kvlist(name.ToLower), TClassKeyValue(Of T)).AddValue(value) Then
                Throw New Exception(String.Format("Invalid parameter {0} value", name))
            End If
        Else
            Throw New Exception(String.Format("Invalid parameter name {0}", name))
        End If
    End Sub

    Public Overloads Sub AddValue(ByVal name As String)
        If _kvlist.ContainsKey(name.ToLower) Then
            If Not CType(_kvlist(name.ToLower), TVoidKeyValue).AddValue() Then
                Throw New Exception(String.Format("Invalid parameter {0} value", name))
            End If
        Else
            Throw New Exception(String.Format("Invalid parameter name {0}", name))
        End If
    End Sub


    Public Overloads Sub AddValue(ByVal name As String, ByVal value As String)
        Dim kv As TBaseKeyValue = Nothing
        If _kvlist.TryGetValue(name.ToLower, kv) Then
            Select Case kv.ValueType

                Case TBaseKeyValue.TValueTypes.vtInt
                    If Not CType(kv, TClassKeyValue(Of Integer)).AddValue(Integer.Parse(value)) Then
                        Throw New Exception(String.Format("Invalid parameter {0} value: {1}", name, value))
                    End If
                    Exit Select

                Case TBaseKeyValue.TValueTypes.vtString
                    If Not CType(kv, TClassKeyValue(Of String)).AddValue(value) Then
                        Throw New Exception(String.Format("Invalid parameter {0} value: {1}", name, value))
                    End If
                    Exit Select

                Case Else
                    Throw New Exception(String.Format("Invalid parameter value: {0}", value))

            End Select
        Else
            Throw New Exception(String.Format("Invalid parameter name: {0}", name))
        End If
    End Sub


    Public Overloads Sub AddValue(ByVal name As String, ByVal value As Integer)
        Dim kv As TBaseKeyValue = Nothing
        If _kvlist.TryGetValue(name.ToLower, kv) Then
            Select Case kv.ValueType

                Case TBaseKeyValue.TValueTypes.vtInt
                    If Not CType(kv, TClassKeyValue(Of Integer)).AddValue(value) Then
                        Throw New Exception(String.Format("Invalid parameter {0} value: {1}", name, value))
                    End If

                Case TBaseKeyValue.TValueTypes.vtString
                    If Not CType(kv, TClassKeyValue(Of String)).AddValue(value.ToString()) Then
                        Throw New Exception(String.Format("Invalid parameter {0} value: {1}", name, value))
                    End If

                Case Else
                    Throw New Exception(String.Format("Invalid value: {0} is specified for {1}", value, name))

            End Select
            Return
        End If
        Throw New Exception(String.Format("Invalid parameter name: {0}", name))
    End Sub
#End Region

#Region "Clear Values"

    Public Overloads Sub ClearValue()
        For Each key As String In _kvlist.Keys
            ClearValue(key)
        Next
    End Sub

    Public Overloads Sub ClearValue(ByVal name As String)
        Dim kv As TBaseKeyValue = Nothing
        If _kvlist.TryGetValue(name.ToLower, kv) Then
            kv.Clear()
        Else
            Throw New Exception(String.Format("Invalid parameter name {0}", name))
        End If
    End Sub

#End Region


#Region "Retrieval functions"
    Public Function FindValue(ByVal name As String) As Boolean
        Return _kvlist.ContainsKey(name.ToLower) AndAlso _kvlist(name.ToLower).Initialized
    End Function

    Protected Function GetClassValue(Of T)(ByVal name As String, Optional ByVal index As Integer = 0) As T
        If FindValue(name) Then
            Return CType(_kvlist(name.ToLower), TClassKeyValue(Of T)).GetValue(index)
        End If
        Throw New Exception(String.Format("Invalid parameter name {0}", name))
    End Function

    Public Function GetInt(ByVal name As String, Optional ByVal index As Integer = 0) As Integer
        Return GetClassValue(Of Integer)(name, index)
    End Function

    Public Function GetString(ByVal name As String, Optional ByVal index As Integer = 0) As String
        Return GetClassValue(Of String)(name, index)
    End Function

    Public Function Count(ByVal name As String) As Integer
        Dim kv As TBaseKeyValue = Nothing
        If _kvlist.TryGetValue(name.ToLower, kv) Then
            Return kv.Count
        End If
        Return 0
    End Function
#End Region

#Region "Test correctness"
    Public Function Validate() As Boolean
        Dim e As Dictionary(Of String, TBaseKeyValue).Enumerator = _kvlist.GetEnumerator
        While e.MoveNext
            With e.Current.Value
                If .Required And Not .Initialized Then
                    Return False
                End If
            End With
        End While
        Return True
    End Function
#End Region

End Class



Public Class TCommandLineParser : Inherits TKeyValueList

#Region "Local Variables"
    Private Shared _separatorChars As String = "/-~@$%^&"
    Private _separatorChar As String = String.Empty
#End Region

#Region "Ctors"

    Public Sub New(Optional ByVal sepChar As String = "/")
        MyBase.New(True)
        If sepChar.Length() = 1 Then
            If _separatorChars.IndexOf(sepChar) <> -1 Then
                _separatorChar = sepChar
            Else
                Throw New Exception("Illegal parameter separator!")
            End If
        Else
            Throw New Exception("Invalid parameter separator!")
        End If
    End Sub

#End Region

#Region "Methods"

    Public Sub AddItems(ByVal args() As String)
        For Each arg As String In args
            AddItem(arg)
        Next
    End Sub


    Public Sub AddItem(ByVal argument As String)
        If argument.StartsWith(_separatorChar) Then
            Dim posEq As Integer = argument.IndexOf("=")
            If posEq <> -1 And posEq > 1 Then
                Dim key As String = argument.Substring(1, posEq - 1)
                If _kvlist.ContainsKey(key.ToLower) Then
                    Dim value As String = argument.Substring(posEq + 1)
                    Me.AddValue(key, value)
                Else
                    Throw New Exception(String.Format("Invalid parameter name: {0}", key))
                End If
            Else
                Me.AddValue(argument.Substring(1))
            End If
        Else
            Throw New Exception(String.Format("Invalid argument: {0}", argument))
        End If
    End Sub


#End Region

End Class
