namespace pzl {
    export function setFrameContent() {
        var ve = Xrm.Page.getAttribute("new_customerprofileid") as Xrm.Page.LookupAttribute;
        if (ve != null && ve.getValue() != null){
            var lookup = ve.getValue()[0];
            var xhr = new XMLHttpRequest();
            xhr.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.0/new_customerprofiles(" + lookup.id.substr(1, 36) + ")/new_campaignurl");
            xhr.send();
            var result = JSON.parse(xhr.response);
            if (result.Value != null){
                var frame = Xrm.Page.getControl("IFRAME_veframe") as Xrm.Page.FramedControl;
                frame.setSrc(result.Value);
            }
        }
    }
}