namespace agur {
    const brregApi = "https://data.brreg.no/enhetsregisteret/enhet/";

    export function getBrregDetails(context: Xrm.Page.EventContext) {
        if (context === null || context === undefined) {
            return;
        }
        var accountNumberAtt = Xrm.Page.getAttribute("accountnumber") as Xrm.Page.StringAttribute;
        if (accountNumberAtt !== null && accountNumberAtt.getIsDirty()) {
            var accountNumber = accountNumberAtt.getValue();
            if (accountNumber !== null) {
                accountNumber = accountNumber.replace(" ", "");
                if (accountNumber.length === 9 && Number(accountNumber) !== NaN) {
                    var xhr = new XMLHttpRequest();
                    xhr.open("GET", brregApi + accountNumber + ".json");
                    xhr.onreadystatechange = function () {
                        if (xhr.readyState === XMLHttpRequest.DONE) {
                            if (xhr.status === 200) {
                                updateAccount(xhr.response);
                            }
                            Xrm.Page.data.entity.removeOnSave(getBrregDetails);
                            Xrm.Page.data.save();
                        }
                    };
                    xhr.send();
                    context.getEventArgs().preventDefault();
                }
            }
        }
    }

    function updateAccount(companyInfo: string) {
        let company = JSON.parse(companyInfo) as Brreg.BrregCompany;
        Xrm.Page.getAttribute("name").setValue(company.navn);

        if (company.forretningsadresse !== null) {
            updateAddress("1", company.forretningsadresse);
        }
        if (company.postadresse !== null) {
            updateAddress("2", company.postadresse);
        }
    }

    function updateAddress(addressNumber: string, address: Brreg.Adresse) {
        Xrm.Page.getAttribute("address" + addressNumber + "_line1").setValue(address.adresse);
        Xrm.Page.getAttribute("address" + addressNumber + "_city").setValue(address.kommune);
        Xrm.Page.getAttribute("address" + addressNumber + "_postalcode").setValue(address.postnummer);
        Xrm.Page.getAttribute("address" + addressNumber + "_country").setValue(address.land);
    }

    Xrm.Page.data.entity.addOnSave(getBrregDetails);
}