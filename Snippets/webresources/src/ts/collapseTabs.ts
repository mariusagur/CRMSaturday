namespace pzl {
    export function collapseTabs(ctx: Xrm.Page.EventContext) {
        var context = ctx.getFormContext();
        var checkBoxes: Xrm.Page.Attribute[] = [];
        if (ctx.getEventArgs() === null) {
            checkBoxes = context.getAttribute((att) => {
                return att.getName().indexOf("collapsebox") !== -1 && att.getValue() === true;
            });
        }
        else {
            checkBoxes = context.getAttribute((att) => {
                return att.getName().indexOf("collapsebox") !== -1
                        && att.getValue() === true
                        && att.getIsDirty() === true;
            })
        }

        checkBoxes.forEach(element => {
            var control = element.controls.get(0);
            var section = control.getParent();
            var tab = section.getParent();
            tab.setDisplayState("collapsed");
        });
    }
}