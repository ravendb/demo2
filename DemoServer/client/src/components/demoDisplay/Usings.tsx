import * as React from "react";
import { Language } from "../../models/commonModels";
import { CodePreview } from "../helpers/CodePreview";
import { Collapse } from "../helpers/Collapse";

interface UsingsProps {
    language: Language;
}

interface UsingsState {
    expand: boolean;
}

export class Usings extends React.Component<UsingsProps, UsingsState> {
    collapseButton: HTMLElement;

    constructor(props) {
        super(props);
        this.state = { expand: false };

        this.toggleCollapse = this.toggleCollapse.bind(this);
    }

    getTitle() {
        const { language } = this.props;
        switch (language) {
            case "csharp":
                return "usings";
            case "java":
            case "python":
                return "imports";
            default:
                return "usings";
        }
    }

    toggleCollapse() {
        this.setState(prevState => ({
            expand: !prevState.expand
        }));
    }

    render() {
        const { language, children } = this.props;
        const { expand } = this.state;
        const buttonClass = expand
            ? "folding"
            : "folding collapsed";

        return <div>
            <a className={buttonClass} role="button" onClick={this.toggleCollapse}>
                {this.getTitle()}
            </a>
            <Collapse id="includes" show={expand}>
                <CodePreview id="preview-usings" language={language}>
                    {children}
                </CodePreview>
            </Collapse>
        </div>;
    }
}