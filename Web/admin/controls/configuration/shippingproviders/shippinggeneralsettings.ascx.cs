#region dashCommerce License
/*
dashCommerce® is Copyright © 2008-2012 Mettle Systems LLC. All Rights Reserved.


dashCommerce, and the dashCommerce logo are registered trademarks of Mettle Systems LLC. Mettle Systems LLC logos and trademarks may not be used without prior written consent.

dashCommerce is licensed under the following license. If you do not accept the terms, please discontinue the use of dashCommerce and uninstall dashCommerce. 

Your license to the dashCommerce source and/or binaries is governed by the Reciprocal Public License 1.5 (RPL1.5) license as described here: 

http://www.opensource.org/licenses/rpl1.5.txt 

If you do not wish to release the source of software you build using dashCommerce, you may purchase a site license, which will allow you to deploy dashCommerce for use in 1 web store defined as using 1 URL. You may purchase a site license here: 

http://www.dashcommerce.org/license.html 
*/
#endregion
using System;
using System.Web.UI;
using MettleSystems.dashCommerce.Core;
using MettleSystems.dashCommerce.Core.Caching;
using MettleSystems.dashCommerce.Localization;
using MettleSystems.dashCommerce.Store;
using MettleSystems.dashCommerce.Store.Services.ShippingService;
using MettleSystems.dashCommerce.Store.Web.Controls;

namespace MettleSystems.dashCommerce.Web.admin.controls.configuration.shippingproviders {
  public partial class shippinggeneralsettings : ShippingConfigurationControl {

    #region Member Variables
    
    ShippingServiceSettings shippingServiceSettings;
    
    #endregion
    
    #region Page Events

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e) {
      try {
        SetShippingGeneralSettingsProperties();
        shippingServiceSettings = ShippingService.FetchConfiguredShippingProviders();
        if(shippingServiceSettings == null) {
          shippingServiceSettings = new ShippingServiceSettings();
        }
        if(!Page.IsPostBack) {
          LoadCountries();
          chkUseShipping.Checked = shippingServiceSettings.UseShipping;
          txtShipFromZip.Text = shippingServiceSettings.ShipFromZip;
          ddlShipFromCountry.SelectedValue = shippingServiceSettings.ShipFromCountryCode;
          txtShippingBuffer.Text = StoreUtility.GetFormattedAmount(shippingServiceSettings.ShippingBuffer, false);
        }
      }
      catch(Exception ex) {
        Logger.Error(typeof(shippinggeneralsettings).Name + ".Page_Load", ex);
        base.MasterPage.MessageCenter.DisplayCriticalMessage(ex.Message);
      }
    }

    /// <summary>
    /// Handles the Click event of the btnSave control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
    protected void btnSave_Click(object sender, EventArgs e) {
      try {
        shippingServiceSettings.UseShipping = chkUseShipping.Checked;
        shippingServiceSettings.ShipFromZip = txtShipFromZip.Text.Trim();
        shippingServiceSettings.ShipFromCountryCode = ddlShipFromCountry.SelectedValue;
        decimal buffer = 0.00M;
        decimal.TryParse(txtShippingBuffer.Text.Trim(), out buffer);
        shippingServiceSettings.ShippingBuffer = buffer;
        int id = base.Save(shippingServiceSettings, WebUtility.GetUserName());
        if(id > 0) {
          MasterPage.MessageCenter.DisplaySuccessMessage(LocalizationUtility.GetText("lblShippingSettingsSaved"));
        }
        else {
          MasterPage.MessageCenter.DisplayFailureMessage(LocalizationUtility.GetText("lblShippingSettingsNotSaved"));
        }
      }
      catch(Exception ex) {
        Logger.Error(typeof(shippinggeneralsettings).Name + ".btnSave_Click", ex);
        base.MasterPage.MessageCenter.DisplayCriticalMessage(ex.Message);
      }     
    }
    
    #endregion
    
    #region Methods
    
    #region Private

    /// <summary>
    /// Loads the countries.
    /// </summary>
    private void LoadCountries() {
      CountryCollection countryCollection = new CountryController().FetchAll().OrderByAsc("Name");
      ddlShipFromCountry.DataSource = countryCollection;
      ddlShipFromCountry.DataValueField = "Code";
      ddlShipFromCountry.DataTextField = "Name";
      ddlShipFromCountry.DataBind();
    }

    /// <summary>
    /// Sets the shipping general settings properties.
    /// </summary>
    private void SetShippingGeneralSettingsProperties() {
      lblShippingBufferCurrencySymbol.Text = SiteSettingCache.GetSiteSettings().CurrencySymbol;
    }
    
    #endregion
    
    #endregion
    
  }
}
