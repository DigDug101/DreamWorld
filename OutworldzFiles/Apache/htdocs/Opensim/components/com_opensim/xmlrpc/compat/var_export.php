<?php
// +----------------------------------------------------------------------+
// | PHP Version 4                                                        |
// +----------------------------------------------------------------------+
// | Copyright (c) 1997-2004 The PHP Group                                |
// +----------------------------------------------------------------------+
// | This source file is subject to version 3.0 of the PHP license,       |
// | that is bundled with this package in the file LICENSE, and is        |
// | available at through the world-wide-web at                           |
// | http://www.php.net/license/3_0.txt.                                  |
// | If you did not receive a copy of the PHP license and are unable to   |
// | obtain it through the world-wide-web, please send a note to          |
// | license@php.net so we can mail you a copy immediately.               |
// +----------------------------------------------------------------------+
// | Authors: Aidan Lister <aidan@php.net>                                |
// +----------------------------------------------------------------------+
//
// $Id: var_export.php 2 2009-03-16 20:22:51Z ggiunta $

/*
 * @component jOpenSim Component
 * @copyright Copyright (C) 2016 FoTo50 http://www.jopensim.com/
 * @license http://www.gnu.org/copyleft/gpl.html GNU General Public License version 2 or later;
 */


/**
 *
 * @category    PHP
 * @package     PHP_Compat
 * @author      Aidan Lister <aidan@php.net>
 * @version     $Revision: 1.2 $
 * @since       PHP 4.2.0
 * @require     PHP 4.0.0 (user_error)
 */

// No direct access to this file
defined('_JEXEC') or die('Restricted Access');

if (!function_exists('var_export')) {
    function var_export($array, $return = false, $lvl=0)
    {
        // Common output variables
        $indent      = '  ';
        $doublearrow = ' => ';
        $lineend     = ",\n";
        $stringdelim = '\'';

        // Check the export isn't a simple string / int
        if (is_string($array)) {
            $out = $stringdelim . str_replace('\'', '\\\'', str_replace('\\', '\\\\', $array)) . $stringdelim;
        } elseif (is_int($array) || is_float($array)) {
            $out = (string)$array;
        } elseif (is_bool($array)) {
            $out = $array ? 'true' : 'false';
        } elseif (is_null($array)) {
            $out = 'NULL';
        } elseif (is_resource($array)) {
            $out = 'resource';
        } else {
            // Begin the array export
            // Start the string
            $out = "array (\n";

            // Loop through each value in array
            foreach ($array as $key => $value) {
                // If the key is a string, delimit it
                if (is_string($key)) {
                    $key = str_replace('\'', '\\\'', str_replace('\\', '\\\\', $key));
                    $key = $stringdelim . $key . $stringdelim;
                }

                $val = var_export($value, true, $lvl+1);

                // Piece together the line
                for ($i = 0; $i < $lvl; $i++)
                    $out .= $indent;
                $out .= $key . $doublearrow . $val . $lineend;
            }

            // End our string
            for ($i = 0; $i < $lvl; $i++)
                $out .= $indent;
            $out .= ")";
        }

        // Decide method of output
        if ($return === true) {
            return $out;
        } else {
            echo $out;
            return;
        }
    }
}
?>