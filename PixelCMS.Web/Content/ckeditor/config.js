/**
 * Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function( config ) {
  // Just put the name of your custom skin here.
  //config.skin = 'moono-light';
    config.skin = 'moonocolor';

    // The toolbar groups arrangement, optimized for two toolbar rows.
    config.toolbarGroups = [
		{ name: 'clipboard', groups: ['clipboard', 'undo'] },
		{ name: 'editing', groups: ['find', 'selection'] },
		{ name: 'links' },

		{ name: 'document', groups: ['mode', 'document', 'doctools'] },
		{ name: 'others' },
		{ name: 'tools' },
		
        { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi'] },
        { name: 'insert' },
        { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		{ name: 'styles' },
		{ name: 'colors' }
    ];
    config.font_defaultLabel = 'Myriad Pro';
    config.font_names =
        'Myriad Pro;' +
        'Arial/Arial, Helvetica, sans-serif;' +
        'Times New Roman/Times New Roman, Times, serif;' +
        'Verdana;';
        'Segoe UI';

    // Remove some buttons, provided by the standard plugins, which we don't
    // need to have in the Standard(s) toolbar.
    config.removeButtons = 'Underline,Subscript,Superscript';

    // Se the most common block elements.
    config.format_tags = 'p;h1;h2;h3;pre';

    // Make dialogs simpler.
    config.removeDialogTabs = 'image:advanced;link:advanced';


    config.filebrowserBrowseUrl = "../../../Content/ckeditor/ckfinder/ckfinder.html";
    config.filebrowserImageBrowseUrl = "../../../Content/ckeditor/ckfinder/ckfinder.html?type=Im­ages";
    config.filebrowserFlashBrowseUrl = "../../../Content/ckeditor/ckfinder/ckfinder.html?type=Fl­ash";
    config.filebrowserUploadUrl = "../../../Content/ckeditor/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Fil­es";
    config.filebrowserImageUploadUrl = "../../../Content/ckeditor//ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Images";
    config.filebrowserFlashUploadUrl = "../../../Content/ckeditor/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Fla­sh";

    config.stylesSet = 'my_styles';

};

CKEDITOR.stylesSet.add('my_styles', [
    // Block-level styles.
    { name: 'Blue Title', element: 'h2', attributes: { 'class': 'my_style' } },
    { name: 'Red Title', element: 'h3', styles: { color: 'Red' } },

    // Inline styles.
    { name: 'CSS Style', element: 'span', attributes: { 'class': 'my_style' } },
    { name: 'Marker: Yellow', element: 'span', styles: { 'background-color': 'Yellow' } }
]);
