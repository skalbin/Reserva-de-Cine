// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


/*AGREGO CÓDIGO PARA VINCULAR PELICULA CON LISTA DESPLEGABLE DE GENEROS*/
$("body").on("change", "#ddlGenero", function () {
     $("input[name=GeneroId]").val($(this).find("option:selected").val());
});

$("body").on("change", "#ddlPelicula", function () {
    $("input[name=PeliculaId]").val($(this).find("option:selected").val());
});

$("body").on("change", "#ddlSala", function () {
    $("input[name=SalaId]").val($(this).find("option:selected").val());
});
$("body").on("change", "#ddlTipoSala", function () {
    $("input[name=TipoSalaId]").val($(this).find("option:selected").val());
});
$("body").on("change", "#ddlFuncion", function () {
    $("input[name=FuncionId]").val($(this).find("option:selected").val());
});


