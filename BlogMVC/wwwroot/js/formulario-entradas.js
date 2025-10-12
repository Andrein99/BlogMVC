const quill = new Quill('#editor', {
    modules: {
        toolbar: [
            [{ header: [1, 2, false] }],
            ['bold', 'italic', 'underline'],
            ['code-block'],
        ],
    },
    placeholder: 'Coloque aquí la entrada...',
    theme: 'snow', // or 'bubble'
});

function cargarContenido(contenido) {
    quill.setContents(contenido, 'silent');
}

function btnEnviarClick() {
    let esValido = validarFormularioCompleto();

    if (!esValido) {
        return;
    }

    const delta = quill.getContents();
    const deltaJSON = JSON.stringify(delta.ops); // Transformación a un string para poder enviarlo al backend

    $("#Cuerpo").val(deltaJSON); // Le damos el valor que tiene el editor de Quill al input oculto, y así podemos tener la funcionalidad de Quill con nuestra propia funcionalidad.
    $("#formEntrada").trigger("submit");
}

function validarFormularioCompleto() {
    let formEntradaEsValido = $("#formEntrada").valid();
    let cuerpoEsValido = validarCuerpo();

    return formEntradaEsValido && cuerpoEsValido;
}

function validarCuerpo() {
    let mensajeDeError = null;
    let esValido = true;

    const htmlCuerpo = quill.getSemanticHTML();

    if (htmlCuerpo === '<p></p>') { /* La documentación de Quill dice que aunque parezca vacío de texto, podría haber un <p> tag  */
        mensajeDeError = "El cuerpo de la entrada es requerido";
        esValido = false;
    }

    $("#cuerpo-error").html(mensajeDeError);
    return esValido;
}

quill.on('text-change', function (delta, oldDelta, source) { // Función que permite sacar un error en la vista si se borra toda la entrada, para mostrar que es requerida. Se simula el comportamiento de la validación que sí se tiene para el campo título, porque Quill es un componente externo.
    validarCuerpo();
})

function mostrarPrevisualizacion(event) {
    const input = event.target;
    const imagenPreview = document.getElementById('PreviewImagen');

    if (input.files && input.files[0]) {
        const urlImagen = URL.createObjectURL(input.files[0]);
        imagenPreview.src = urlImagen;
        imagenPreview.style.display = 'block';
    }
}