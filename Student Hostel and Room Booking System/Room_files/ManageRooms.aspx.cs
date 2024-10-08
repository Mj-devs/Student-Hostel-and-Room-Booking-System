﻿using Microsoft.EntityFrameworkCore;
using Student_Hostel_and_Room_Booking_System.Models.Datalayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Student_Hostel_and_Room_Booking_System
{
    public partial class ManageRooms : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {   
                // Retrieving the RoomCoordinatoor Id from the session
                if (Session["RoomCoordinatorId"] != null)
                {
                    int RoomCoordinatorId = (int)Session["RoomCoordinatorId"];

                    using (var context = new StudentHostelDBContext())
                    {
                        var RoomCoordinator = context.RoomCoordinator.Where(r => RoomCoordinatorId == r.RoomCoordinatorId).FirstOrDefault();
                        if (RoomCoordinator != null)
                        {
                            lbluser.Text = $"User that is signed in: {RoomCoordinator.Username}";
                        }
                        else
                        {
                            // Handle the case where the RoomCoordinator is not found
                            lbluser.Text = "Coordinator not found";
                        }
                    }
                }
                else
                {
                    // Case where session is null (redirect to login page)
                    Response.Redirect("~/Login.aspx");
                }
                BindRoomsGrid();
            }
        }

        private void BindRoomsGrid()
        {
            using (var context = new StudentHostelDBContext())
            {
                var rooms = context.Rooms.Include("Hostel").ToList();
                RoomsGridView.DataSource = rooms;
                RoomsGridView.DataBind();
            }
        }
        protected void RoomsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                // Get the selected room ID
                int roomId = Convert.ToInt32(e.CommandArgument);

                // Store the RoomId in a session variable
                Session["SelectedRoomId"] = roomId;

                // Redirect to the RoomDetails page
                Response.Redirect("/Room_files/ViewRoomDetails.aspx");
            }
        }

        protected void RoomsGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            RoomsGridView.EditIndex = e.NewEditIndex;
            BindRoomsGrid();
        }

        protected void RoomsGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            RoomsGridView.EditIndex = -1;
            BindRoomsGrid();
        }

        protected void RoomsGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            // Get the Room ID of the room being updated from the DataKeys collection of the GridView.
            int roomId = Convert.ToInt32(RoomsGridView.DataKeys[e.RowIndex].Value);

            // Get the GridViewRow corresponding to the row that is being updated.
            GridViewRow row = RoomsGridView.Rows[e.RowIndex];

            // Retrieve the controls for Room Number, Hostel DropDown, Room Type, Bed Spaces, Available Bed Spaces, and IsAvailable checkbox.
            TextBox roomNumber = (TextBox)(row.Cells[1].Controls[0]); 
            int hostelId = int.Parse((row.FindControl("ddlHostel") as DropDownList).SelectedValue);
            TextBox roomType = (TextBox)(row.Cells[3].Controls[0]);
            TextBox bedSpacesTextBox = (TextBox)(row.Cells[4].Controls[0]);
            TextBox availableBedSpacesTextBox = (TextBox)(row.Cells[5].Controls[0]);
            CheckBox isAvailable = (CheckBox)(row.Cells[6].Controls[0]);

            using (var context = new StudentHostelDBContext())
            {
                // Find the room by its ID in the database.
                var room = context.Rooms.Find(roomId);

                // Get the new number of bed spaces entered by the user.
                int newBedSpaces = int.Parse(bedSpacesTextBox.Text);

                // Store the current number of available bed spaces for the room.
                int currentAvailableBedSpaces = room.AvailableBedSpaces;

                // Update the available bed spaces based on the change in the number of bed spaces.
                // If the new number of bed spaces is less than the current bed spaces, decrease available bed spaces.
                if (newBedSpaces < room.BedSpaces)
                {
                    room.AvailableBedSpaces -= (room.BedSpaces - newBedSpaces);
                }
                else
                {
                    // Otherwise, increase the available bed spaces.
                    room.AvailableBedSpaces += (newBedSpaces - room.BedSpaces);
                }

                // Update the room's properties with the new values from the form.
                room.RoomNumber = roomNumber.Text; 
                room.HostelId = hostelId; 
                room.BedSpaces = newBedSpaces; 
                room.AvailableBedSpaces = currentAvailableBedSpaces; 
                room.IsAvailable = isAvailable.Checked;

                // Save the changes to the database.
                context.SaveChanges();

                // Reset the EditIndex property of the GridView to exit edit mode.
                RoomsGridView.EditIndex = -1;

                // Refresh the displayed data with updated information.
                BindRoomsGrid();
            }
        }

        protected void ErrorTimer_Tick(object sender, EventArgs e)
        {
            lblMessage.Visible = false; // Hide the error message
            ErrorTimer.Enabled = false;
        }

        protected void RoomsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int roomId = Convert.ToInt32(RoomsGridView.DataKeys[e.RowIndex].Value);

            using (var context = new StudentHostelDBContext())
            {
                var room = context.Rooms.Find(roomId);

                var bookingsExist = context.Bookings.Any(b => b.RoomId == roomId);
                if (bookingsExist)
                {
                    lblMessage.Text = "Cannot delete room. There are student booked to the room. Kindly first unbook them before deleting the room.";
                    lblMessage.Visible = true;

                    // Reset and start the timer
                    ErrorTimer.Enabled = true;
                   
                }
                else
                {
                    if (room != null)
                    {
                        context.Rooms.Remove(room);
                        context.SaveChanges();
                        lblMessage.Text = "Room deleted successfully!";
                    }
                }
            }

            BindRoomsGrid();
        }

        protected void RoomsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var _context = new StudentHostelDBContext();
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.RowState.HasFlag(DataControlRowState.Edit))
            {
                // Find the DropDownList in the EditItemTemplate
                var ddlHostel = e.Row.FindControl("ddlHostel") as DropDownList;

                if (ddlHostel != null)
                {
                    // Get the list of hostels from the database
                    var hostels = _context.Hostels.ToList();

                    // Bind the DropDownList to the hostel data
                    ddlHostel.DataSource = hostels;
                    ddlHostel.DataTextField = "HostelName";  
                    ddlHostel.DataValueField = "HostelId";   
                    ddlHostel.DataBind();

                    // Set the selected value to match the current student record
                    var currentHostelId = DataBinder.Eval(e.Row.DataItem, "HostelId").ToString();
                    ddlHostel.SelectedValue = currentHostelId;
                }
            }

        }
        protected void btnAddRoom_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Room_files/AddRoom.aspx");
        }
        protected void btnBookRoom_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Room_files/BookRoom.aspx");
        }
        protected void btnSearchRoom_Click(object sender, EventArgs e)
        {
            using (var context = new StudentHostelDBContext())
            {
                // Get the search query from the text box
                string searchQuery = txtSearchRoom.Text.Trim();

                // Check if the search query is not empty
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    // Perform the search query to filter rooms by RoomNumber
                    var rooms = context.Rooms
                        .Where(r => r.RoomNumber.Contains(searchQuery))
                        .ToList();

                    // Bind the filtered rooms to the GridView
                    RoomsGridView.DataSource = rooms;
                    RoomsGridView.DataBind();
                }
                else
                {
                    BindRoomsGrid();
                }
            }
        }


    }
}