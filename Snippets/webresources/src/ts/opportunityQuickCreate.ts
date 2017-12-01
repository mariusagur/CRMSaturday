// JavaScript source code
export function quickCreateOpportunity() {
    class Customer {
        public Id: string;
        public Name: string;
        public Type: string;
        constructor(Customer?: Xrm.Page.LookupValue) {
            this.Id = Customer.id;
            this.Name = Customer.name;
            this.Type = Customer.entityType;
        }

        getOpenArguments() {
            return {
                customerid: this.Id,
                customeridname: this.Name,
                customeridtype: this.Type
            }
        }
    }

    var customer = new Customer();

    switch (Xrm.Page.data.entity.getEntityName().toLowerCase()) {
        case "account":
        case "contact":
            customer.Name = Xrm.Page.data.entity.getEntityName();
            customer.Id = Xrm.Page.data.entity.getId();
            customer.Type = Xrm.Page.data.entity.getPrimaryAttributeValue();
            break;
        case "opportunity":
        case "incident":
            try {
                if (Xrm.Page.getAttribute("customerid") != null) {
                    var contact = Xrm.Page.getAttribute("customerid").getValue()[0] as Xrm.Page.LookupValue;
                    customer = new Customer(contact);
                }
            }
            catch (ex) { }
            break;
        default:
            break;
    }

    Xrm.Page.getAttribute()

    Xrm.Utility.openQuickCreate("opportunity", null, customer.getOpenArguments());
}