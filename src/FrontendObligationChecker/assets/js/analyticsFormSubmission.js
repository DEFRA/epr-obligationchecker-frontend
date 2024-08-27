function singleQuestionFormSubmission(event) {
    let formData = {
        "event": "single-question-form-submission"
    };

    let target = event.target

    for (let i = 0; i < target.length; i++) {
        let element = target.elements[i]

        if (element.type === "radio" && element.checked) {
            let elementName = element.getAttribute("name")
            formData[elementName] = element.value
        }
    }

    window.dataLayer = window.dataLayer || [];

    window.dataLayer.push(formData);
}

let singleQuestionForm = document.getElementsByClassName("single-question-form")

if (singleQuestionForm && singleQuestionForm.length === 1) {
    singleQuestionForm[0].addEventListener("submit", singleQuestionFormSubmission)
}