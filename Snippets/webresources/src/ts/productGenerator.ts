"use strict";

$(function () {
    pzl.showAllProducts();
});

namespace pzl {
    var productTypeValue = 803880002;
    var dueDate = new Date();
    dueDate.setDate(dueDate.getDate() + 7);
    /**
     * Gets all the selected products and creates new child opportunities
     */
    export function generateOpportunities() {
        var opportunityId = parent.Xrm.Page.data.entity.getId().substring(1, 37);
        var followUpDate = (parent.Xrm.Page.getAttribute("schedulefollowup_qualify") != null) ? parent.Xrm.Page.getAttribute("schedulefollowup_qualify").getValue() as Date : null;
        var account = (parent.Xrm.Page.getAttribute("parentaccountid") != null) ? parent.Xrm.Page.getAttribute("parentaccountid").getValue() as Array<Xrm.Page.LookupValue> : null;
        var contact = (parent.Xrm.Page.getAttribute("parentcontactid") != null) ? parent.Xrm.Page.getAttribute("parentcontactid").getValue() as Array<Xrm.Page.LookupValue> : null;
        var selectedProducts = $(":checked");
        selectedProducts.each(function (i: number) {
            var element = $(this).parent();
            var myProduct = new Product(
                element.text(),
                element.parent().prop("id"),
                opportunityId,
                followUpDate,
                account !== null && account.length > 0 ? account[0] : null,
                contact !== null && contact.length > 0 ? contact[0] : null,
                `"${dueDate.getFullYear()}-${dueDate.getMonth() + 1}-${dueDate.getDate()}`
                );
            createProductOpportunity(myProduct);
        });
    }

    /**
     * Gets all the product names from the mcf_category option set and generates checkbox inputs for them
     */
    export function showAllProducts() {
        var categories = new Array<Product>();
        var id = parent.Xrm.Page.data.entity.getId().substr(1, 36);
        var categoryOptions = (parent.Xrm.Page.getAttribute("mcf_category") as Xrm.Page.OptionSetAttribute).getOptions();
        categoryOptions.forEach(c => {
            if (c.value >= 0) {
                categories.push(new Product(c.text, c.value, id, null, null, null, null));
            }
        });
        showCheckBoxes(categories);
    }

    /**
     * Takes an array of products and generates checkbox controls to the page by cloning the template
     * @param productNames The products to be listed
     */
    function showCheckBoxes(products: Array<Product>) {
        if (products.length > 0) {
            var checkBoxTemplate = $("#myCloneableCheckBox").first();
            products.forEach(n => {
                var newProduct = checkBoxTemplate.clone();
                newProduct.removeClass("hide");
                newProduct.prop("id", n.category.toString());
                newProduct.children().html(newProduct.children().html() + n.name);
                newProduct.appendTo($("#products"));
            });
            $("#createOpportunitiesButton").removeClass("hide");
        }
    }

    /**
     * Creates a new child opportunity in MSDYN365
     * @param product The product to be created
     */
    function createProductOpportunity(product: Product) {
        var xhr = new XMLHttpRequest();
        var xrmVersion = parent.Xrm.Page.context.getVersion().substr(0, 3);
        xhr.open("POST", parent.Xrm.Page.context.getClientUrl() + "/api/data/v" + xrmVersion + "/opportunities");
        xhr.onload = function (e: object) {
            if (xhr.readyState === 4) {
                if (xhr.status === 200 || xhr.status === 204) {
                    refreshPage();
                } else {
                    console.error(xhr.statusText);
                    window.prompt("An error occurred, please provide your system administrator with the following error", xhr.responseText);
                }
            }
        };
        xhr.setRequestHeader("OData-MaxVersion", "4.0");
        xhr.setRequestHeader("OData-Version", "4.0");
        xhr.setRequestHeader("Accept", "application/json");
        xhr.setRequestHeader("Content-Type", "application/json; odata.metadata=full; charset=utf-8");
        xhr.send(product.Stringify());
    }

    /**
     * Refreshes the RichUX iframe and resets the checkboxes
     */
    function refreshPage() {
        var richUxControl = parent.Xrm.Page.ui.controls.get("WebResource_richux") as Xrm.Page.FramedControl;
        var richUxSrc = richUxControl.getSrc();
        richUxControl.setSrc("about:blank");
        richUxControl.setSrc(richUxSrc);
        $(":checked").prop("checked", false);
    }

    /**
     * Product class which can be stringified to JSON for creation in MSDYN
     */
    class Product {
        constructor(
            public name: string,
            public category: number,
            public parentOpportunity: string,
            public followUpDate: Date,
            public account: Xrm.Page.LookupValue,
            public contact: Xrm.Page.LookupValue,
            public dueDate: string,
        ) {
        }
        Stringify(): string {
            var json =
                `{
                "name": "${this.name}",
                "mcf_category": ${this.category},
                "mcf_parentopportunity@odata.bind": "/opportunities(${this.parentOpportunity})",
                "schedulefollowup_qualify": ${JSON.stringify(this.followUpDate)},
                "mcf_opportunitytype": ${productTypeValue},
                "estimatedclosedate": ${this.dueDate}`;
            if (this.account !== null) {
                json += `,
                    "parentaccountid@odata.bind": "/accounts(${this.account.id.substr(1, 36)})"`;
            }
            if (this.contact !== null) {
                json += `,
                    "parentcontactid@odata.bind": "/contacts(${this.contact.id.substr(1, 36)})"`;
            }
            json += "}";
            return json;
        }
    }
}