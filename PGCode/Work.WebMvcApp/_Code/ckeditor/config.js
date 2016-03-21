/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function( config )
{
	// Define changes to default configuration here. For example:
	config.language = 'zh';
	config.uiColor = '#AADC6E';
    config.stylesSet=
    [
     { name: '大標題', element: 'h3' },
     { name: '小標題1', element: 'h4' },
     { name: '小標題2', element: 'h5' },
     { name: '段落', element: 'p' }
    ];
	
	config.contentsCss = ['../../Content/css/editor.css'];

};
