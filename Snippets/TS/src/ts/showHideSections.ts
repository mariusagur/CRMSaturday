namespace agur {
    /**
     * Function to show or hide sections on a form
     * @param sectionName The name of the section(s) 
     * @param showSection Boolean flag to set visibility to true or false
     */
    function setSectionVisibility(sectionName: string, showSection: boolean) {
        // Gets all tabs which includes at least one section with the given name
        //  checks whether Xrm is defined to accomodate controlling visibility
        //  through web resources and frames
        var tabs: Xrm.Page.Tab[];
        if (Xrm !== null) {
            tabs = Xrm.Page.ui.tabs.get(t => {
                return t.sections.get(sectionName) !== null
            });
        } else {
            tabs = parent.Xrm.Page.ui.tabs.get(t => {
                return t.sections.get(sectionName) !== null
            });
        }
        // Sets sets the visibility of all sections with the given name in all tabs
        tabs.forEach(t => {
            t.sections.get(sectionName).setVisible(showSection);
        });
    }

    /**
     * Hides all sections with the given name
     * @param sectionName The name of the section(s) to hide
     */
    export function hideSections(sectionName: string){
        setSectionVisibility(sectionName, false);
    }
    /**
     * Shows all sections with the given name
     * @param sectionName The name of the section(s) to show
     */
    export function showSections(sectionName: string){
        setSectionVisibility(sectionName, true);
    }
}