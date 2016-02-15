<%@ Page Language="C#" AutoEventWireup="true"  %>

<script runat="server">
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect(IntranetPublic.Properties.Settings.Default.BaseURL + "/getfile.aspx?id_file=" + Request.QueryString["id_file"]);
        }
</script>