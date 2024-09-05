﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddStudents.aspx.cs" Inherits="Student_Hostel_and_Room_Booking_System.AddStudents" %><asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">    <header>        <style>            .form-control {                margin-bottom: 15px;             }        </style>    </header>    <!--Form Add Student-->    <div>    <asp:TextBox ID="txtname" runat="server" placeholder="Name" CssClass="form-control"></asp:TextBox>    <asp:Panel ID="pnlTextBoxContainer" runat="server">        <asp:TextBox ID="txtmatricno" runat="server" placeholder="Matric Number" CssClass="form-control" />    </asp:Panel>    <asp:TextBox ID="txtphonenumber" runat="server" placeholder="Phone Number" CssClass="form-control"></asp:TextBox>    <asp:TextBox ID="txtgender" runat="server" placeholder="Gender" CssClass="form-control"></asp:TextBox>    <asp:Button ID="btnsave" runat="server" Text="Save" OnClick="btnsave_Click" CssClass="btn btn-primary" />    </div>    <br />     <div>            <!-- LinkButton to toggle visibility -->            <asp:LinkButton ID="lbToggleFields" runat="server" Text="Add Fresher +" CausesValidation="false" OnClick="lbToggleFields_Click" CssClass="btn btn-primary" />            <!-- Panel for hidden fields -->             <br />            <asp:Panel ID="pnlHiddenFields" runat="server" CssClass="form-group" Visible="false">         <div>             <br />            <asp:TextBox ID="txtjambno" runat="server" placeholder="Jamb Registration No." CssClass="form-control"></asp:TextBox><br />                         <!-- Customvalidator to check matric no same-->            <asp:CustomValidator                 ID="cvMatricNo"                 runat="server"                 ControlToValidate="txtmatricno"                 OnServerValidate="cvMatricNo_ServerValidate"                 ErrorMessage="Matriculation number already exists."                 Display="Dynamic"                ValidationGroup="SaveGroup"                ForeColor="Red">            </asp:CustomValidator>                        <!--Save button-->            <asp:Button ID="btnsavefresher" runat="server"  Text="Save"                         CssClass=" btn btn-primary h-25 w-25"                         OnClick="btnsavefresher_Click" ValidationGroup="SaveGroup" />                </div>            </asp:Panel>        </div></asp:Content>