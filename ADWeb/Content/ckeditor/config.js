/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */
CKEDITOR.editorConfig = function( config ) {
    config.toolbar = 'Custom';

    config.toolbar_Custom = [
        { name: 'styles', items: ['Font', 'FontSize'] },
        { name: 'basicstyles', items: ['Bold', 'Italic', 'Strike', 'Underline' ] },
        { name: 'paragraph', items: ['NumberedList', 'BulletedList'] },
        { name: 'colors', items: ['RemoveFormat', 'TextColor', 'BGColor'] },
        { name: 'tools', items: ['Source'] }
    ];

    config.fontSize_style = {
        element: 'span',
        styles: { 'font-size': '#12pt' },
        overrides: [{ element: 'font', attributes: { 'size': null }}]
    };
};
