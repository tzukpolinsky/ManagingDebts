export interface Menu {
    Text: string;
    SubMenus?: Array<SubMenu>;
    Href?: string;
    icon: string;
    }
export interface SubMenu {
        Text: string;
        Href: string;
    }
