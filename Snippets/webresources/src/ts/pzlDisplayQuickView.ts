namespace pzl {
    export function showQuickView() {
        var qvControls = Xrm.Page.ui.tabs.get(t => {
            return t.getName().indexOf("VEQV") === 0;
        });

        qvControls.forEach(t => {
            t.setVisible(true);
        })
    }
}